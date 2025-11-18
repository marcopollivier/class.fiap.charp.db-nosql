using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.DynamoDemo.Models;

/// <summary>
/// Modelo Pedido para DynamoDB usando single-table design
/// PK = CLIENTE#{ClienteId}, SK = PEDIDO#{PedidoId}
/// </summary>
public class Pedido : DynamoEntityBase
{
    [DynamoDBProperty("pedido_id")]
    [Required]
    public string PedidoId { get; set; } = string.Empty;

    [DynamoDBProperty("cliente_id")]
    [Required]
    public int ClienteId { get; set; }

    [DynamoDBProperty("data_pedido")]
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    [DynamoDBProperty("valor_total")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor total deve ser maior que zero")]
    public decimal ValorTotal { get; set; }

    [DynamoDBProperty("status")]
    [Required]
    public StatusPedido Status { get; set; } = StatusPedido.PENDENTE;

    /// <summary>
    /// Observações do pedido
    /// </summary>
    [DynamoDBProperty("observacoes")]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Construtor padrão para DynamoDB
    /// </summary>
    public Pedido()
    {
        Tipo = "PEDIDO";
    }

    /// <summary>
    /// Construtor com parâmetros
    /// </summary>
    public Pedido(string pedidoId, int clienteId, decimal valorTotal) : this()
    {
        PedidoId = pedidoId;
        ClienteId = clienteId;
        ValorTotal = valorTotal;
        SetKeys();
    }

    /// <summary>
    /// CONCEPT-EXPLANATION: Configura as chaves para o pedido
    /// PK = CLIENTE#{ClienteId}, SK = PEDIDO#{PedidoId}
    /// </summary>
    public void SetKeys()
    {
        PK = $"CLIENTE#{ClienteId}";
        SK = $"PEDIDO#{PedidoId}";
    }

    /// <summary>
    /// Atualiza o status do pedido
    /// </summary>
    public void AtualizarStatus(StatusPedido novoStatus)
    {
        Status = novoStatus;
        MarkAsUpdated();
    }

    /// <summary>
    /// Adiciona observação ao pedido
    /// </summary>
    public void AdicionarObservacao(string observacao)
    {
        Observacoes = string.IsNullOrEmpty(Observacoes)
            ? observacao
            : $"{Observacoes}; {observacao}";
        MarkAsUpdated();
    }

    public void MarkAsUpdated()
    {
        AtualizadoEm = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return $"Pedido {PedidoId}: Cliente {ClienteId} - R${ValorTotal:F2} ({Status})";
    }
}

/// <summary>
/// Enum para status dos pedidos
/// </summary>
public enum StatusPedido
{
    PENDENTE,
    CONFIRMADO,
    PAGO,
    PREPARANDO,
    ENVIADO,
    ENTREGUE,
    CANCELADO
}
