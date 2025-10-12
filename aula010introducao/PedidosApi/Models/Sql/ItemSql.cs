using System.ComponentModel.DataAnnotations;

namespace PedidosApi.Models.Sql;

public class ItemSql
{
    [Key]
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
}
