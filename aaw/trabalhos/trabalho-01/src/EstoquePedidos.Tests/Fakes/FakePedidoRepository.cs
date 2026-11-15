using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Repositories;
using MongoDB.Bson;

namespace EstoquePedidos.Tests.Fakes;

public class FakePedidoRepository : IPedidoRepository
{
    private readonly List<Pedido> _pedidos = new();

    public Task<PagedResult<Pedido>> ListarAsync(string? status, int page, int pageSize)
    {
        IEnumerable<Pedido> query = _pedidos;

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(p =>
                string.Equals(p.Status, status.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        var filtrados = query.OrderByDescending(p => p.CriadoEm).ToList();
        var pagina = Math.Max(1, page);
        var tamanho = Math.Clamp(pageSize, 1, 100);
        var itens = filtrados
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToList();

        return Task.FromResult(new PagedResult<Pedido>(itens, filtrados.Count, pagina, tamanho));
    }

    public Task<List<Pedido>> ListarPorProdutoIdAsync(string produtoId)
    {
        if (!ObjectId.TryParse(produtoId, out _))
        {
            return Task.FromResult(new List<Pedido>());
        }

        var itens = _pedidos
            .Where(p => p.ProdutoId == produtoId)
            .OrderByDescending(p => p.CriadoEm)
            .ToList();

        return Task.FromResult(itens);
    }

    public Task<Pedido?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return Task.FromResult<Pedido?>(null);
        }

        var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(pedido);
    }

    public Task<Pedido> CriarAsync(Pedido pedido)
    {
        if (string.IsNullOrWhiteSpace(pedido.Id))
        {
            pedido.Id = ObjectId.GenerateNewId().ToString();
        }

        _pedidos.Add(pedido);
        return Task.FromResult(pedido);
    }

    public Task<bool> AtualizarAsync(string id, Pedido pedido)
    {
        var indice = _pedidos.FindIndex(p => p.Id == id);
        if (indice < 0)
        {
            return Task.FromResult(false);
        }

        pedido.Id = id;
        _pedidos[indice] = pedido;
        return Task.FromResult(true);
    }

    public Task<bool> ExistePedidoParaProdutoAsync(string produtoId)
    {
        if (!ObjectId.TryParse(produtoId, out _))
        {
            return Task.FromResult(false);
        }

        var existe = _pedidos.Any(p => p.ProdutoId == produtoId);
        return Task.FromResult(existe);
    }
}
