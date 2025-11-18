using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.DynamoDemo.Models;

/// <summary>
/// Classe base para entidades DynamoDB usando Single-Table Design Pattern
/// PK agrupa entidades relacionadas, SK identifica tipo e instância
/// </summary>
[DynamoDBTable("PedidosApp")]
public abstract class DynamoEntityBase
{
    [DynamoDBHashKey]
    [Required]
    public string PK { get; set; } = string.Empty;

    [DynamoDBRangeKey]
    [Required]
    public string SK { get; set; } = string.Empty;

    [DynamoDBProperty]
    [Required]
    public string Tipo { get; set; } = string.Empty;

    [DynamoDBProperty]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    [DynamoDBProperty]
    public DateTime? AtualizadoEm { get; set; }
}
