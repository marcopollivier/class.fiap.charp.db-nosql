using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using ClientApp.DynamoDemo.Models;
using Microsoft.Extensions.Logging;

namespace ClientApp.DynamoDemo.Services;

public class DynamoDbService : IDynamoDbService
{
    private readonly IDynamoDBContext _context;
    private readonly IAmazonDynamoDB _client;
    private readonly ILogger<DynamoDbService> _logger;
    private const string TableName = "PedidosApp";

    public DynamoDbService(IDynamoDBContext context, IAmazonDynamoDB client, ILogger<DynamoDbService> logger)
    {
        _context = context;
        _client = client;
        _logger = logger;
    }

    public async Task<Cliente?> ObterClienteAsync(int clienteId)
    {
        try
        {
            var cliente = await _context.LoadAsync<Cliente>($"CLIENTE#{clienteId}", $"CLIENTE#{clienteId}");
            return cliente;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente {ClienteId}", clienteId);
            return null;
        }
    }

    public async Task<bool> InserirClienteAsync(Cliente cliente)
    {
        try
        {
            cliente.SetKeys();
            await _context.SaveAsync(cliente);
            _logger.LogInformation("✅ Cliente {ClienteId} inserido com sucesso", cliente.ClienteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inserir cliente {ClienteId}", cliente.ClienteId);
            return false;
        }
    }

    public async Task<bool> AtualizarClienteAsync(Cliente cliente)
    {
        try
        {
            cliente.MarkAsUpdated();
            await _context.SaveAsync(cliente);
            _logger.LogInformation("✅ Cliente {ClienteId} atualizado", cliente.ClienteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cliente {ClienteId}", cliente.ClienteId);
            return false;
        }
    }

    public async Task<bool> RemoverClienteAsync(int clienteId)
    {
        try
        {
            await _context.DeleteAsync<Cliente>($"CLIENTE#{clienteId}", $"CLIENTE#{clienteId}");
            _logger.LogInformation("✅ Cliente {ClienteId} removido", clienteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover cliente {ClienteId}", clienteId);
            return false;
        }
    }

    public async Task<List<Cliente>> ListarTodosClientesAsync()
    {
        try
        {
            var search = _context.ScanAsync<Cliente>(new ScanCondition[]
            {
                new ScanCondition("Tipo", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, "CLIENTE")
            });
            return await search.GetRemainingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar clientes");
            return new List<Cliente>();
        }
    }

    public async Task<Profile?> ObterProfileAsync(int clienteId)
    {
        try
        {
            var profile = await _context.LoadAsync<Profile>($"CLIENTE#{clienteId}", "PROFILE");
            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter profile do cliente {ClienteId}", clienteId);
            return null;
        }
    }

    public async Task<bool> InserirProfileAsync(Profile profile)
    {
        try
        {
            profile.SetKeys();
            await _context.SaveAsync(profile);
            _logger.LogInformation("✅ Profile do cliente {ClienteId} inserido", profile.ClienteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inserir profile do cliente {ClienteId}", profile.ClienteId);
            return false;
        }
    }

    public async Task<bool> AtualizarProfileAsync(Profile profile)
    {
        try
        {
            profile.MarkAsUpdated();
            await _context.SaveAsync(profile);
            _logger.LogInformation("✅ Profile do cliente {ClienteId} atualizado", profile.ClienteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar profile do cliente {ClienteId}", profile.ClienteId);
            return false;
        }
    }

    public async Task<Pedido?> ObterPedidoAsync(int clienteId, string pedidoId)
    {
        try
        {
            var pedido = await _context.LoadAsync<Pedido>($"CLIENTE#{clienteId}", $"PEDIDO#{pedidoId}");
            return pedido;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido {PedidoId} do cliente {ClienteId}", pedidoId, clienteId);
            return null;
        }
    }

    public async Task<bool> InserirPedidoAsync(Pedido pedido)
    {
        try
        {
            pedido.SetKeys();
            await _context.SaveAsync(pedido);
            _logger.LogInformation("✅ Pedido {PedidoId} inserido para cliente {ClienteId}",
                pedido.PedidoId, pedido.ClienteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inserir pedido {PedidoId}", pedido.PedidoId);
            return false;
        }
    }

    public async Task<bool> AtualizarPedidoAsync(Pedido pedido)
    {
        try
        {
            pedido.MarkAsUpdated();
            await _context.SaveAsync(pedido);
            _logger.LogInformation("✅ Pedido {PedidoId} atualizado", pedido.PedidoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar pedido {PedidoId}", pedido.PedidoId);
            return false;
        }
    }

    public async Task<bool> RemoverPedidoAsync(int clienteId, string pedidoId)
    {
        try
        {
            await _context.DeleteAsync<Pedido>($"CLIENTE#{clienteId}", $"PEDIDO#{pedidoId}");
            _logger.LogInformation("✅ Pedido {PedidoId} removido", pedidoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover pedido {PedidoId}", pedidoId);
            return false;
        }
    }

    public async Task<List<Pedido>> ListarPedidosClienteAsync(int clienteId)
    {
        try
        {
            var search = _context.QueryAsync<Pedido>($"CLIENTE#{clienteId}", Amazon.DynamoDBv2.DocumentModel.QueryOperator.BeginsWith, new object[] { "PEDIDO#" });
            return await search.GetRemainingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos do cliente {ClienteId}", clienteId);
            return new List<Pedido>();
        }
    }

    public async Task<Item?> ObterItemAsync(int clienteId, string pedidoId, int itemId)
    {
        try
        {
            var item = await _context.LoadAsync<Item>($"CLIENTE#{clienteId}", $"PEDIDO#{pedidoId}#ITEM#{itemId}");
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter item {ItemId} do pedido {PedidoId}", itemId, pedidoId);
            return null;
        }
    }

    public async Task<bool> InserirItemAsync(Item item)
    {
        try
        {
            item.SetKeys();
            await _context.SaveAsync(item);
            _logger.LogInformation("✅ Item {ItemId} inserido no pedido {PedidoId}",
                item.ItemId, item.PedidoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inserir item {ItemId}", item.ItemId);
            return false;
        }
    }

    public async Task<bool> AtualizarItemAsync(Item item)
    {
        try
        {
            item.MarkAsUpdated();
            await _context.SaveAsync(item);
            _logger.LogInformation("✅ Item {ItemId} atualizado", item.ItemId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar item {ItemId}", item.ItemId);
            return false;
        }
    }

    public async Task<bool> RemoverItemAsync(int clienteId, string pedidoId, int itemId)
    {
        try
        {
            await _context.DeleteAsync<Item>($"CLIENTE#{clienteId}", $"PEDIDO#{pedidoId}#ITEM#{itemId}");
            _logger.LogInformation("✅ Item {ItemId} removido", itemId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover item {ItemId}", itemId);
            return false;
        }
    }

    public async Task<List<Item>> ListarItensDosPedidosAsync(int clienteId)
    {
        try
        {
            var search = _context.ScanAsync<Item>(new ScanCondition[]
            {
                new ScanCondition("PK", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, $"CLIENTE#{clienteId}"),
                new ScanCondition("Tipo", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, "ITEM")
            });
            return await search.GetRemainingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar itens do cliente {ClienteId}", clienteId);
            return new List<Item>();
        }
    }

    public async Task<List<Item>> ListarItensDoPedidoAsync(int clienteId, string pedidoId)
    {
        try
        {
            var search = _context.QueryAsync<Item>($"CLIENTE#{clienteId}", Amazon.DynamoDBv2.DocumentModel.QueryOperator.BeginsWith, new object[] { $"PEDIDO#{pedidoId}#ITEM#" });
            return await search.GetRemainingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar itens do pedido {PedidoId}", pedidoId);
            return new List<Item>();
        }
    }

    public async Task<Endereco?> ObterEnderecoAsync(int clienteId, TipoEndereco tipoEndereco)
    {
        try
        {
            var endereco = await _context.LoadAsync<Endereco>($"CLIENTE#{clienteId}", $"ENDERECO#{tipoEndereco}");
            return endereco;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter endereço {TipoEndereco} do cliente {ClienteId}", tipoEndereco, clienteId);
            return null;
        }
    }

    public async Task<bool> InserirEnderecoAsync(Endereco endereco)
    {
        try
        {
            endereco.SetKeys();
            await _context.SaveAsync(endereco);
            _logger.LogInformation("✅ Endereço {TipoEndereco} inserido para cliente {ClienteId}",
                endereco.TipoEndereco, endereco.ClienteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inserir endereço {TipoEndereco}", endereco.TipoEndereco);
            return false;
        }
    }

    public async Task<bool> AtualizarEnderecoAsync(Endereco endereco)
    {
        try
        {
            endereco.MarkAsUpdated();
            await _context.SaveAsync(endereco);
            _logger.LogInformation("✅ Endereço {TipoEndereco} atualizado", endereco.TipoEndereco);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar endereço {TipoEndereco}", endereco.TipoEndereco);
            return false;
        }
    }

    public async Task<bool> RemoverEnderecoAsync(int clienteId, TipoEndereco tipoEndereco)
    {
        try
        {
            await _context.DeleteAsync<Endereco>($"CLIENTE#{clienteId}", $"ENDERECO#{tipoEndereco}");
            _logger.LogInformation("✅ Endereço {TipoEndereco} removido", tipoEndereco);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover endereço {TipoEndereco}", tipoEndereco);
            return false;
        }
    }

    public async Task<List<Endereco>> ListarEnderecosClienteAsync(int clienteId)
    {
        try
        {
            var search = _context.QueryAsync<Endereco>($"CLIENTE#{clienteId}", Amazon.DynamoDBv2.DocumentModel.QueryOperator.BeginsWith, new object[] { "ENDERECO#" });
            return await search.GetRemainingAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar endereços do cliente {ClienteId}", clienteId);
            return new List<Endereco>();
        }
    }

    public async Task<List<DynamoEntityBase>> ObterTodosItensClienteAsync(int clienteId)
    {
        try
        {
            // Esta consulta requer uso direto do client pois retorna tipos mistos
            var request = new QueryRequest
            {
                TableName = TableName,
                KeyConditionExpression = "PK = :pk",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [":pk"] = new($"CLIENTE#{clienteId}")
                }
            };

            var response = await _client.QueryAsync(request);
            var items = new List<DynamoEntityBase>();

            // Mapear de volta para modelos tipados baseado no tipo
            foreach (var item in response.Items)
            {
                var tipo = item["Tipo"].S;
                switch (tipo)
                {
                    case "CLIENTE":
                        var cliente = await _context.LoadAsync<Cliente>($"CLIENTE#{clienteId}", $"CLIENTE#{clienteId}");
                        if (cliente != null) items.Add(cliente);
                        break;
                    case "PROFILE":
                        var profile = await _context.LoadAsync<Profile>($"CLIENTE#{clienteId}", "PROFILE");
                        if (profile != null) items.Add(profile);
                        break;
                    case "PEDIDO":
                        var pedidoId = item["pedido_id"].S;
                        var pedido = await _context.LoadAsync<Pedido>($"CLIENTE#{clienteId}", $"PEDIDO#{pedidoId}");
                        if (pedido != null) items.Add(pedido);
                        break;
                    // Adicionar outros tipos conforme necessário
                }
            }

            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os itens do cliente {ClienteId}", clienteId);
            return new List<DynamoEntityBase>();
        }
    }

    public async Task<Dictionary<string, int>> ObterEstatisticasPorTipoAsync()
    {
        try
        {
            var request = new ScanRequest
            {
                TableName = TableName,
                ProjectionExpression = "Tipo"
            };

            var response = await _client.ScanAsync(request);

            return response.Items
                .GroupBy(item => item["Tipo"].S)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas por tipo");
            return new Dictionary<string, int>();
        }
    }

    public async Task<bool> VerificarTabelaExistenteAsync()
    {
        try
        {
            var response = await _client.DescribeTableAsync(TableName);
            return response.Table.TableStatus == TableStatus.ACTIVE;
        }
        catch (ResourceNotFoundException)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar tabela {TableName}", TableName);
            return false;
        }
    }
}
