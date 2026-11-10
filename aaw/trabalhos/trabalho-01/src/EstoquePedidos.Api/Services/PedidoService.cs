using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Exceptions;
using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Repositories;
using MongoDB.Bson;

namespace EstoquePedidos.Api.Services;

public class PedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
    }

    public decimal CalcularDesconto(decimal subtotal, int quantidade, string? cupom)
    {
        var temCupomDesc10 = false;

        if (!string.IsNullOrWhiteSpace(cupom))
        {
            var cupomNormalizado = cupom.Trim();
            if (!string.Equals(cupomNormalizado, "DESC10", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Cupom promocional invalido.");
            }

            temCupomDesc10 = true;
        }

        if (temCupomDesc10 || quantidade >= 10)
        {
            return Math.Round(subtotal * 0.10m, 2);
        }

        return 0m;
    }

    public decimal CalcularFrete(decimal valorComDesconto)
    {
        return valorComDesconto >= 100.00m ? 0m : 15.00m;
    }

    public async Task<PagedResult<PedidoResponse>> ListarAsync(
        string? status = null,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            throw new ArgumentException("O numero da pagina deve ser maior ou igual a 1.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentException("O tamanho da pagina deve estar entre 1 e 100.");
        }

        var resultado = await _pedidoRepository.ListarAsync(status, page, pageSize);
        var itensDto = resultado.Items.Select(MapearParaResponse).ToList();

        return new PagedResult<PedidoResponse>(
            itensDto,
            resultado.Total,
            resultado.Page,
            resultado.PageSize);
    }

    public async Task<List<PedidoResponse>> ListarPorProdutoIdAsync(string produtoId)
    {
        if (string.IsNullOrWhiteSpace(produtoId) || !ObjectId.TryParse(produtoId, out _))
        {
            throw new NaoEncontradoException($"Produto com identificador '{produtoId}' nao foi encontrado.");
        }

        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
        if (produto is null)
        {
            throw new NaoEncontradoException($"Produto com identificador '{produtoId}' nao foi encontrado.");
        }

        var pedidos = await _pedidoRepository.ListarPorProdutoIdAsync(produtoId);
        return pedidos.Select(MapearParaResponse).ToList();
    }

    public async Task<PedidoResponse> ObterPorIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
        {
            throw new NaoEncontradoException($"Pedido com identificador '{id}' nao foi encontrado.");
        }

        var pedido = await _pedidoRepository.ObterPorIdAsync(id);
        if (pedido is null)
        {
            throw new NaoEncontradoException($"Pedido com identificador '{id}' nao foi encontrado.");
        }

        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse> CriarAsync(CriarPedidoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NomeCliente))
        {
            throw new ArgumentException("O nome do cliente e obrigatorio.");
        }

        if (request.Quantidade <= 0)
        {
            throw new ArgumentException("A quantidade do pedido deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(request.ProdutoId) || !ObjectId.TryParse(request.ProdutoId, out _))
        {
            throw new NaoEncontradoException($"Produto com identificador '{request.ProdutoId}' nao foi encontrado.");
        }

        var produto = await _produtoRepository.ObterPorIdAsync(request.ProdutoId);
        if (produto is null)
        {
            throw new NaoEncontradoException($"Produto com identificador '{request.ProdutoId}' nao foi encontrado.");
        }

        if (!produto.Ativo)
        {
            throw new ConflitoException($"O produto '{produto.Nome}' esta inativo e nao aceita novos pedidos.");
        }

        if (produto.Estoque < request.Quantidade)
        {
            throw new ConflitoException(
                $"Estoque insuficiente para o produto '{produto.Nome}'. Disponivel: {produto.Estoque}, solicitado: {request.Quantidade}.");
        }

        var subtotal = Math.Round(produto.Preco * request.Quantidade, 2);
        var desconto = CalcularDesconto(subtotal, request.Quantidade, request.Cupom);
        var valorComDesconto = subtotal - desconto;
        var frete = CalcularFrete(valorComDesconto);
        var valorTotal = Math.Round(valorComDesconto + frete, 2);

        var cupomNormalizado = string.IsNullOrWhiteSpace(request.Cupom)
            ? null
            : request.Cupom.Trim().ToUpperInvariant();

        produto.Estoque -= request.Quantidade;
        await _produtoRepository.AtualizarAsync(produto.Id, produto);

        var novoPedido = new Pedido
        {
            ProdutoId = produto.Id,
            NomeProduto = produto.Nome,
            NomeCliente = request.NomeCliente.Trim(),
            Quantidade = request.Quantidade,
            Cupom = cupomNormalizado,
            Subtotal = subtotal,
            Desconto = desconto,
            Frete = frete,
            ValorTotal = valorTotal,
            Status = "Confirmado",
            CriadoEm = DateTime.UtcNow
        };

        var criado = await _pedidoRepository.CriarAsync(novoPedido);
        return MapearParaResponse(criado);
    }

    public async Task<PedidoResponse> CancelarAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
        {
            throw new NaoEncontradoException($"Pedido com identificador '{id}' nao foi encontrado.");
        }

        var pedido = await _pedidoRepository.ObterPorIdAsync(id);
        if (pedido is null)
        {
            throw new NaoEncontradoException($"Pedido com identificador '{id}' nao foi encontrado.");
        }

        if (string.Equals(pedido.Status, "Cancelado", StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflitoException("Este pedido ja foi cancelado anteriormente.");
        }

        var produto = await _produtoRepository.ObterPorIdAsync(pedido.ProdutoId);
        if (produto is not null)
        {
            produto.Estoque += pedido.Quantidade;
            await _produtoRepository.AtualizarAsync(produto.Id, produto);
        }

        pedido.Status = "Cancelado";
        await _pedidoRepository.AtualizarAsync(pedido.Id, pedido);

        return MapearParaResponse(pedido);
    }

    private static PedidoResponse MapearParaResponse(Pedido pedido) =>
        new(
            pedido.Id,
            pedido.ProdutoId,
            pedido.NomeProduto,
            pedido.NomeCliente,
            pedido.Quantidade,
            pedido.Cupom,
            pedido.Subtotal,
            pedido.Desconto,
            pedido.Frete,
            pedido.ValorTotal,
            pedido.Status,
            pedido.CriadoEm);
}
