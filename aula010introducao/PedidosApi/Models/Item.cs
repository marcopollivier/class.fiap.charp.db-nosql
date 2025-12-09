using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PedidosApi.Models;

public class Item
{
    [BsonId]
    public string? Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
}
