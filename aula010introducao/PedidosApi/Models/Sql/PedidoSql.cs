using System.ComponentModel.DataAnnotations;
namespace PedidosApi.Models.Sql;

public class PedidoSql
{
    [Key]
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataPedido { get; set; } = DateTime.Now;
    public List<ItemSql> Itens { get; set; } = new();
}
