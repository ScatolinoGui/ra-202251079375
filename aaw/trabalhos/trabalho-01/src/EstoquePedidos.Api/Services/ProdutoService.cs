using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Exceptions;
using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Repositories;
using MongoDB.Bson;

namespace EstoquePedidos.Api.Services;

public class ProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public ProdutoService(
        IProdutoRepository produtoRepository,
        IPedidoRepository pedidoRepository)
    {
        _produtoRepository = produtoRepository;
        _pedidoRepository = pedidoRepository;
    }

    public async Task<PagedResult<ProdutoResponse>> ListarAsync(
        string? categoria = null,
        string? busca = null,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            throw new ArgumentException("O número da página deve ser maior ou igual a 1.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentException("O tamanho da página deve estar entre 1 e 100.");
        }

        var resultado = await _produtoRepository.ListarAsync(categoria, busca, page, pageSize);
        var itensDto = resultado.Items.Select(MapearParaResponse).ToList();

        return new PagedResult<ProdutoResponse>(
            itensDto,
            resultado.Total,
            resultado.Page,
            resultado.PageSize);
    }

    public async Task<ProdutoResponse> ObterPorIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
        {
            throw new NaoEncontradoException($"Produto com identificador '{id}' não foi encontrado.");
        }

        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto is null)
        {
            throw new NaoEncontradoException($"Produto com identificador '{id}' não foi encontrado.");
        }

        return MapearParaResponse(produto);
    }

    public async Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request)
    {
        ValidarDadosProduto(request.Nome, request.Categoria, request.Preco, request.Estoque);

        var nomeNormalizado = request.Nome.Trim();
        var categoriaNormalizada = request.Categoria.Trim();

        var existente = await _produtoRepository.ObterPorNomeAsync(nomeNormalizado);
        if (existente is not null)
        {
            throw new ConflitoException($"Já existe um produto cadastrado com o nome '{nomeNormalizado}'.");
        }

        var novoProduto = new Produto
        {
            Nome = nomeNormalizado,
            Categoria = categoriaNormalizada,
            Preco = Math.Round(request.Preco, 2),
            Estoque = request.Estoque,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        var criado = await _produtoRepository.CriarAsync(novoProduto);
        return MapearParaResponse(criado);
    }

    public async Task<ProdutoResponse> AtualizarAsync(string id, AtualizarProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
        {
            throw new NaoEncontradoException($"Produto com identificador '{id}' não foi encontrado.");
        }

        var atual = await _produtoRepository.ObterPorIdAsync(id);
        if (atual is null)
        {
            throw new NaoEncontradoException($"Produto com identificador '{id}' não foi encontrado.");
        }

        ValidarDadosProduto(request.Nome, request.Categoria, request.Preco, request.Estoque);

        var nomeNormalizado = request.Nome.Trim();
        var categoriaNormalizada = request.Categoria.Trim();

        var existenteComMesmoNome = await _produtoRepository.ObterPorNomeAsync(nomeNormalizado);
        if (existenteComMesmoNome is not null && existenteComMesmoNome.Id != id)
        {
            throw new ConflitoException($"Já existe outro produto cadastrado com o nome '{nomeNormalizado}'.");
        }

        atual.Nome = nomeNormalizado;
        atual.Categoria = categoriaNormalizada;
        atual.Preco = Math.Round(request.Preco, 2);
        atual.Estoque = request.Estoque;
        atual.Ativo = request.Ativo;

        await _produtoRepository.AtualizarAsync(id, atual);
        return MapearParaResponse(atual);
    }

    public async Task RemoverAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !ObjectId.TryParse(id, out _))
        {
            throw new NaoEncontradoException($"Produto com identificador '{id}' não foi encontrado.");
        }

        var existente = await _produtoRepository.ObterPorIdAsync(id);
        if (existente is null)
        {
            throw new NaoEncontradoException($"Produto com identificador '{id}' não foi encontrado.");
        }

        var possuiPedidos = await _pedidoRepository.ExistePedidoParaProdutoAsync(id);
        if (possuiPedidos)
        {
            throw new ConflitoException("Não é permitido excluir um produto que já possui pedidos vinculados.");
        }

        await _produtoRepository.RemoverAsync(id);
    }

    private static void ValidarDadosProduto(string nome, string categoria, decimal preco, int estoque)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do produto é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(categoria))
        {
            throw new ArgumentException("A categoria do produto é obrigatória.");
        }

        if (preco <= 0)
        {
            throw new ArgumentException("O preço do produto deve ser maior que zero.");
        }

        if (estoque < 0)
        {
            throw new ArgumentException("O estoque do produto não pode ser negativo.");
        }
    }

    private static ProdutoResponse MapearParaResponse(Produto produto) =>
        new(
            produto.Id,
            produto.Nome,
            produto.Categoria,
            produto.Preco,
            produto.Estoque,
            produto.Ativo,
            produto.CriadoEm);
}
