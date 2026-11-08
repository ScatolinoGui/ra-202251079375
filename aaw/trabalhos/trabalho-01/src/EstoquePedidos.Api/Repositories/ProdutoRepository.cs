using System.Text.RegularExpressions;
using EstoquePedidos.Api.Config;
using EstoquePedidos.Api.DTOs;
using EstoquePedidos.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EstoquePedidos.Api.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly IMongoCollection<Produto> _collection;

    public ProdutoRepository(MongoDbContext context)
    {
        _collection = context.Produtos;
    }

    public async Task<PagedResult<Produto>> ListarAsync(string? categoria, string? busca, int page, int pageSize)
    {
        var paginaAtual = Math.Max(1, page);
        var tamanhoPagina = Math.Clamp(pageSize, 1, 100);

        var builder = Builders<Produto>.Filter;
        var filtro = builder.Empty;

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            filtro &= builder.Eq(p => p.Categoria, categoria.Trim());
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termoSeguro = Regex.Escape(busca.Trim());
            filtro &= builder.Regex(p => p.Nome, new BsonRegularExpression(termoSeguro, "i"));
        }

        var total = await _collection.CountDocumentsAsync(filtro);

        var itens = await _collection
            .Find(filtro)
            .SortBy(p => p.Nome)
            .Skip((paginaAtual - 1) * tamanhoPagina)
            .Limit(tamanhoPagina)
            .ToListAsync();

        return new PagedResult<Produto>(itens, total, paginaAtual, tamanhoPagina);
    }

    public async Task<Produto?> ObterPorIdAsync(string id)
    {
        // Evita FormatException do driver quando o id da URL não tem 24 hexadecimais de ObjectId
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Produto?> ObterPorNomeAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return null;
        }

        var nomeSeguro = Regex.Escape(nome.Trim());
        var filtro = Builders<Produto>.Filter.Regex(
            p => p.Nome,
            new BsonRegularExpression($"^{nomeSeguro}$", "i"));

        return await _collection.Find(filtro).FirstOrDefaultAsync();
    }

    public async Task<Produto> CriarAsync(Produto produto)
    {
        await _collection.InsertOneAsync(produto);
        return produto;
    }

    public async Task<bool> AtualizarAsync(string id, Produto produto)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return false;
        }

        produto.Id = id;
        var resultado = await _collection.ReplaceOneAsync(p => p.Id == id, produto);
        return resultado.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return false;
        }

        var resultado = await _collection.DeleteOneAsync(p => p.Id == id);
        return resultado.DeletedCount > 0;
    }

    public async Task<long> ContarAsync()
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<Produto>.Empty);
    }
}
