using System.ComponentModel.DataAnnotations;
namespace PedidosApi.Models.Sql;

public class PedidoSql
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public DateTime DataPedido { get; set; } = DateTime.Now;
    public List<ItemSql> Itens { get; set; } = new();
}
