using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EstoquePedidos.Api.Models;

public class Usuario
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    // Perfis aceitos nas políticas RBAC: "Admin" ou "Usuario"
    public string Role { get; set; } = "Usuario";
}
