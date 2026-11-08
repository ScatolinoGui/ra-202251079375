using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EstoquePedidos.Api.Models;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    // Referência 1-N para o documento correspondente na coleção produtos
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProdutoId { get; set; } = string.Empty;

    public string NomeProduto { get; set; } = string.Empty;

    public string NomeCliente { get; set; } = string.Empty;

    public int Quantidade { get; set; }

    public string? Cupom { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Subtotal { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Desconto { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Frete { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ValorTotal { get; set; }

    public string Status { get; set; } = "Confirmado";

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
