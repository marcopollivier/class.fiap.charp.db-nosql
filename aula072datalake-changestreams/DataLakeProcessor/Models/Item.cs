using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLakeProcessor.Models;

/// <summary>
/// Modelo padronizado para Item
/// Usado em todas as aulas do curso para consistência
/// </summary>
public class Item
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;
    
    [BsonElement("preco")]
    public decimal Preco { get; set; }
    
    [BsonElement("quantidade")]
    public int Quantidade { get; set; }
}

/// <summary>
/// Item simplificado usado dentro de Pedidos
/// </summary>
public class ItemPedido
{
    [BsonElement("itemId")]
    public ObjectId ItemId { get; set; }
    
    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;
    
    [BsonElement("preco")]
    public decimal Preco { get; set; }
    
    [BsonElement("quantidade")]
    public int Quantidade { get; set; }
}