using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Models;
using EstoquePedidos.Api.Repositories;
using MongoDB.Bson;

namespace EstoquePedidos.Tests.Fakes;

public class FakeProdutoRepository : IProdutoRepository
{
    private readonly List<Produto> _produtos = new();

    public Task<PagedResult<Produto>> ListarAsync(string? categoria, string? busca, int page, int pageSize)
    {
        IEnumerable<Produto> query = _produtos;

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(p =>
                string.Equals(p.Categoria, categoria.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            query = query.Where(p =>
                p.Nome.Contains(busca.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        var filtrados = query.OrderBy(p => p.Nome).ToList();
        var pagina = Math.Max(1, page);
        var tamanho = Math.Clamp(pageSize, 1, 100);
        var itens = filtrados
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToList();

        return Task.FromResult(new PagedResult<Produto>(itens, filtrados.Count, pagina, tamanho));
    }

    public Task<Produto?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return Task.FromResult<Produto?>(null);
        }

        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(produto);
    }

    public Task<Produto?> ObterPorNomeAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return Task.FromResult<Produto?>(null);
        }

        var produto = _produtos.FirstOrDefault(p =>
            string.Equals(p.Nome, nome.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(produto);
    }

    public Task<Produto> CriarAsync(Produto produto)
    {
        if (string.IsNullOrWhiteSpace(produto.Id))
        {
            produto.Id = ObjectId.GenerateNewId().ToString();
        }

        _produtos.Add(produto);
        return Task.FromResult(produto);
    }

    public Task<bool> AtualizarAsync(string id, Produto produto)
    {
        var indice = _produtos.FindIndex(p => p.Id == id);
        if (indice < 0)
        {
            return Task.FromResult(false);
        }

        produto.Id = id;
        _produtos[indice] = produto;
        return Task.FromResult(true);
    }

    public Task<bool> RemoverAsync(string id)
    {
        var removido = _produtos.RemoveAll(p => p.Id == id) > 0;
        return Task.FromResult(removido);
    }

    public Task<long> ContarAsync()
    {
        return Task.FromResult((long)_produtos.Count);
    }
}
