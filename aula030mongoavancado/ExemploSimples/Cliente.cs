using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExemploSimples;

public class Cliente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
}
