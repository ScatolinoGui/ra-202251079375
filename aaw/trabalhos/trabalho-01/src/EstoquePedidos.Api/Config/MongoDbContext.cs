using EstoquePedidos.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EstoquePedidos.Api.Config;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
        {
            throw new InvalidOperationException("A string de conexão do MongoDB não foi configurada em MongoDbSettings:ConnectionString.");
        }

        if (string.IsNullOrWhiteSpace(settings.DatabaseName))
        {
            throw new InvalidOperationException("O nome do banco do MongoDB não foi configurado em MongoDbSettings:DatabaseName.");
        }

        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<Produto> Produtos => _database.GetCollection<Produto>("produtos");

    public IMongoCollection<Pedido> Pedidos => _database.GetCollection<Pedido>("pedidos");

    public IMongoCollection<Usuario> Usuarios => _database.GetCollection<Usuario>("usuarios");
}
