using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLakeProcessor.Models;

/// <summary>
/// Modelo padronizado para Cliente
/// Usado em todas as aulas do curso para consistência
/// </summary>
public class Cliente
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;
    
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
}