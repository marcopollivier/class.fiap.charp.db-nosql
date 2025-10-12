using System.ComponentModel.DataAnnotations;

namespace PedidosApi.Models.Sql;

public class ClienteSql
{
    [Key]
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
