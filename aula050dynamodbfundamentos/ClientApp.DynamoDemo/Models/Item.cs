using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace ClientApp.DynamoDemo.Models;

/// <summary>
/// Modelo Item para DynamoDB usando hierarchical sort key pattern
/// PK = CLIENTE#{ClienteId}, SK = PEDIDO#{PedidoId}#ITEM#{ItemId}
/// </summary>
public class Item : DynamoEntityBase
{
    [DynamoDBProperty("pedido_id")]
    [Required]
    public string PedidoId { get; set; } = string.Empty;

    [DynamoDBProperty("item_id")]
    [Required]
    public int ItemId { get; set; }

    /// <summary>
    /// ID do cliente (para compor a PK)
    /// </summary>
    [DynamoDBProperty("cliente_id")]
    [Required]
    public int ClienteId { get; set; }

    /// <summary>
    /// Nome/descrição do item
    /// </summary>
    [DynamoDBProperty("nome")]
    [Required]
    [StringLength(200)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade do item no pedido
    /// </summary>
    [DynamoDBProperty("quantidade")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public int Quantidade { get; set; }

    /// <summary>
    /// Preço unitário do item
    /// </summary>
    [DynamoDBProperty("preco_unitario")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
    public decimal PrecoUnitario { get; set; }

    /// <summary>
    /// Valor total calculado (quantidade * preço unitário)
    /// </summary>
    [DynamoDBIgnore]
    public decimal ValorTotal => Quantidade * PrecoUnitario;

    /// <summary>
    /// Código do produto (se aplicável)
    /// </summary>
    [DynamoDBProperty("codigo_produto")]
    public string? CodigoProduto { get; set; }

    /// <summary>
    /// Observações específicas do item
    /// </summary>
    [DynamoDBProperty("observacoes")]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Construtor padrão para DynamoDB
    /// </summary>
    public Item()
    {
        Tipo = "ITEM";
    }

    /// <summary>
    /// Construtor com parâmetros
    /// </summary>
    public Item(int clienteId, string pedidoId, int itemId, string nome, int quantidade, decimal precoUnitario) : this()
    {
        ClienteId = clienteId;
        PedidoId = pedidoId;
        ItemId = itemId;
        Nome = nome;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        SetKeys();
    }

    /// <summary>
    /// CONCEPT-EXPLANATION: Configura as chaves para o item
    /// PK = CLIENTE#{ClienteId}, SK = PEDIDO#{PedidoId}#ITEM#{ItemId}
    /// </summary>
    public void SetKeys()
    {
        PK = $"CLIENTE#{ClienteId}";
        SK = $"PEDIDO#{PedidoId}#ITEM#{ItemId}";
    }

    /// <summary>
    /// Atualiza a quantidade do item
    /// </summary>
    public void AtualizarQuantidade(int novaQuantidade)
    {
        if (novaQuantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero");

        Quantidade = novaQuantidade;
        MarkAsUpdated();
    }

    /// <summary>
    /// Atualiza o preço unitário
    /// </summary>
    public void AtualizarPreco(decimal novoPreco)
    {
        if (novoPreco <= 0)
            throw new ArgumentException("Preço deve ser maior que zero");

        PrecoUnitario = novoPreco;
        MarkAsUpdated();
    }

    public void MarkAsUpdated()
    {
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Validação customizada do item
    /// </summary>
    public bool IsValid(out List<string> errors)
    {
        errors = new List<string>();

        if (string.IsNullOrWhiteSpace(PedidoId))
            errors.Add("PedidoId é obrigatório");

        if (ItemId <= 0)
            errors.Add("ItemId deve ser maior que zero");

        if (string.IsNullOrWhiteSpace(Nome))
            errors.Add("Nome é obrigatório");

        if (Quantidade <= 0)
            errors.Add("Quantidade deve ser maior que zero");

        if (PrecoUnitario <= 0)
            errors.Add("Preço unitário deve ser maior que zero");

        return errors.Count == 0;
    }

    public override string ToString()
    {
        return $"Item {ItemId}: {Nome} - Qtd: {Quantidade} x R${PrecoUnitario:F2} = R${ValorTotal:F2}";
    }
}
