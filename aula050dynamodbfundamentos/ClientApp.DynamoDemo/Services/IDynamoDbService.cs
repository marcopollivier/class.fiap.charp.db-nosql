using ClientApp.DynamoDemo.Models;

namespace ClientApp.DynamoDemo.Services;

/// <summary>
/// EDUCATIONAL-CONTEXT: Interface para operações DynamoDB
///
/// Define o contrato para operações CRUD usando os modelos tipados.
/// Facilita testes unitários e abstrai detalhes de implementação.
/// </summary>
public interface IDynamoDbService
{
    // === Operações Cliente ===
    Task<Cliente?> ObterClienteAsync(int clienteId);
    Task<bool> InserirClienteAsync(Cliente cliente);
    Task<bool> AtualizarClienteAsync(Cliente cliente);
    Task<bool> RemoverClienteAsync(int clienteId);
    Task<List<Cliente>> ListarTodosClientesAsync();

    // === Operações Profile ===
    Task<Profile?> ObterProfileAsync(int clienteId);
    Task<bool> InserirProfileAsync(Profile profile);
    Task<bool> AtualizarProfileAsync(Profile profile);

    // === Operações Pedido ===
    Task<Pedido?> ObterPedidoAsync(int clienteId, string pedidoId);
    Task<bool> InserirPedidoAsync(Pedido pedido);
    Task<bool> AtualizarPedidoAsync(Pedido pedido);
    Task<bool> RemoverPedidoAsync(int clienteId, string pedidoId);
    Task<List<Pedido>> ListarPedidosClienteAsync(int clienteId);

    // === Operações Item ===
    Task<Item?> ObterItemAsync(int clienteId, string pedidoId, int itemId);
    Task<bool> InserirItemAsync(Item item);
    Task<bool> AtualizarItemAsync(Item item);
    Task<bool> RemoverItemAsync(int clienteId, string pedidoId, int itemId);
    Task<List<Item>> ListarItensDosPedidosAsync(int clienteId);
    Task<List<Item>> ListarItensDoPedidoAsync(int clienteId, string pedidoId);

    // === Operações Endereço ===
    Task<Endereco?> ObterEnderecoAsync(int clienteId, TipoEndereco tipoEndereco);
    Task<bool> InserirEnderecoAsync(Endereco endereco);
    Task<bool> AtualizarEnderecoAsync(Endereco endereco);
    Task<bool> RemoverEnderecoAsync(int clienteId, TipoEndereco tipoEndereco);
    Task<List<Endereco>> ListarEnderecosClienteAsync(int clienteId);

    // === Consultas Complexas ===
    Task<List<DynamoEntityBase>> ObterTodosItensClienteAsync(int clienteId);
    Task<Dictionary<string, int>> ObterEstatisticasPorTipoAsync();
    Task<bool> VerificarTabelaExistenteAsync();
}
