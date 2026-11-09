using System.Text.RegularExpressions;
using EstoquePedidos.Api.Config;
using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EstoquePedidos.Api.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly IMongoCollection<Pedido> _collection;

    public PedidoRepository(MongoDbContext context)
    {
        _collection = context.Pedidos;
    }

    public async Task<PagedResult<Pedido>> ListarAsync(string? status, int page, int pageSize)
    {
        var paginaAtual = Math.Max(1, page);
        var tamanhoPagina = Math.Clamp(pageSize, 1, 100);

        var builder = Builders<Pedido>.Filter;
        var filtro = builder.Empty;

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusSeguro = Regex.Escape(status.Trim());
            filtro &= builder.Regex(
                p => p.Status,
                new BsonRegularExpression($"^{statusSeguro}$", "i"));
        }

        var total = await _collection.CountDocumentsAsync(filtro);

        var itens = await _collection
            .Find(filtro)
            .SortByDescending(p => p.CriadoEm)
            .Skip((paginaAtual - 1) * tamanhoPagina)
            .Limit(tamanhoPagina)
            .ToListAsync();

        return new PagedResult<Pedido>(itens, total, paginaAtual, tamanhoPagina);
    }

    public async Task<List<Pedido>> ListarPorProdutoIdAsync(string produtoId)
    {
        if (!ObjectId.TryParse(produtoId, out _))
        {
            return new List<Pedido>();
        }

        return await _collection
            .Find(p => p.ProdutoId == produtoId)
            .SortByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task<Pedido?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Pedido> CriarAsync(Pedido pedido)
    {
        await _collection.InsertOneAsync(pedido);
        return pedido;
    }

    public async Task<bool> AtualizarAsync(string id, Pedido pedido)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return false;
        }

        pedido.Id = id;
        var resultado = await _collection.ReplaceOneAsync(p => p.Id == id, pedido);
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> ExistePedidoParaProdutoAsync(string produtoId)
    {
        if (!ObjectId.TryParse(produtoId, out _))
        {
            return false;
        }

        return await _collection.Find(p => p.ProdutoId == produtoId).AnyAsync();
    }
}
