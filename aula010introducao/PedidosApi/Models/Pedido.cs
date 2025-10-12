using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PedidosApi.Models;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string ClienteId { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; } = DateTime.Now;
    public List<Item> Itens { get; set; } = new();
    public decimal Total => Itens.Sum(i => i.Preco * i.Quantidade);
}
