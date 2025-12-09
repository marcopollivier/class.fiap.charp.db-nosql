using System.ComponentModel.DataAnnotations;
namespace PedidosApi.Models.Sql;

public class ItemSql
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string PedidoId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
}
