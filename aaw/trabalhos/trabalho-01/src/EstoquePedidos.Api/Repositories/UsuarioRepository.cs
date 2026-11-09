using System.Text.RegularExpressions;
using EstoquePedidos.Api.Config;
using EstoquePedidos.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EstoquePedidos.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IMongoCollection<Usuario> _collection;

    public UsuarioRepository(MongoDbContext context)
    {
        _collection = context.Usuarios;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var emailSeguro = Regex.Escape(email.Trim());
        var filtro = Builders<Usuario>.Filter.Regex(
            u => u.Email,
            new BsonRegularExpression($"^{emailSeguro}$", "i"));

        return await _collection.Find(filtro).FirstOrDefaultAsync();
    }

    public async Task<Usuario?> ObterPorIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        usuario.Email = usuario.Email.Trim().ToLowerInvariant();
        await _collection.InsertOneAsync(usuario);
        return usuario;
    }

    public async Task<long> ContarUsuariosAsync()
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<Usuario>.Empty);
    }
}
