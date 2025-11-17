using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataLakeProcessor.Models;

/// <summary>
/// Modelo padronizado para Pedido
/// Usado em todas as aulas do curso para consistência
/// </summary>
public class Pedido
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("clienteId")]
    public ObjectId ClienteId { get; set; }
    
    [BsonElement("dataPedido")]
    public DateTime DataPedido { get; set; }
    
    [BsonElement("itens")]
    public List<ItemPedido> Itens { get; set; } = new();
    
    [BsonElement("total")]
    public decimal Total { get; set; }
}