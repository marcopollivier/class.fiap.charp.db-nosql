namespace PedidosApi.Models;

public class Pedido
{
    public string Id { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; } = DateTime.Now;
    public List<Item> Itens { get; set; } = new();
    public decimal Total => Itens.Sum(i => i.Preco * i.Quantidade);
}
