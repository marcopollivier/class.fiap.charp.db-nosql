namespace PedidosApi.Models;

/// <summary>
/// DTO unificado para criação de clientes - funciona para ambos SQL e NoSQL
/// </summary>
public class ClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO unificado para criação de pedidos
/// </summary>
public class PedidoDto
{
    /// <summary>
    /// ID do cliente no MongoDB (string/ObjectId)
    /// </summary>
    public string ClienteId { get; set; } = string.Empty;

    /// <summary>
    /// ID do cliente no SQL Server (int)
    /// </summary>
    public int SqlClienteId { get; set; }

    public List<ItemDto> Itens { get; set; } = new();
}

/// <summary>
/// DTO para itens de pedido
/// </summary>
public class ItemDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
}
