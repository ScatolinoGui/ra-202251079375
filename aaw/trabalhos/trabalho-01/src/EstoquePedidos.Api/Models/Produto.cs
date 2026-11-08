using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EstoquePedidos.Api.Models;

public class Produto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Categoria { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Preco { get; set; }

    public int Estoque { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
