using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PedidosApi.Models;

public class Cliente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
