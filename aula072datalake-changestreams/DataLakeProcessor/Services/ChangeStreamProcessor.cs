using MongoDB.Driver;
using MongoDB.Bson;
using DataLakeProcessor.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DataLakeProcessor.Services;

/// <summary>
/// Serviço para processar Change Streams do MongoDB e alimentar o Data Lake
/// </summary>
public class ChangeStreamProcessor
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<ChangeStreamProcessor> _logger;
    private readonly IConfiguration _configuration;
    private readonly DataLakeService _dataLakeService;

    public ChangeStreamProcessor(
        IMongoDatabase database,
        ILogger<ChangeStreamProcessor> logger,
        IConfiguration configuration,
        DataLakeService dataLakeService)
    {
        _database = database;
        _logger = logger;
        _configuration = configuration;
        _dataLakeService = dataLakeService;
    }

    /// <summary>
    /// Inicia o monitoramento de Change Streams
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🚀 Iniciando monitoramento de Change Streams...");

        // Monitorar diferentes collections em paralelo
        var tasks = new List<Task>
        {
            MonitorarPedidos(cancellationToken),
            MonitorarClientes(cancellationToken),
            MonitorarItens(cancellationToken)
        };

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Monitora mudanças na collection de pedidos
    /// </summary>
    private async Task MonitorarPedidos(CancellationToken cancellationToken)
    {
        try
        {
            var collection = _database.GetCollection<Pedido>("pedidos");
            
            // Pipeline para filtrar apenas operações relevantes
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Pedido>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                               change.OperationType == ChangeStreamOperationType.Update ||
                               change.OperationType == ChangeStreamOperationType.Delete);

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };

            _logger.LogInformation("📦 Monitorando mudanças em PEDIDOS...");

            using var cursor = await collection.WatchAsync(pipeline, options, cancellationToken);

            await cursor.ForEachAsync(async change =>
            {
                try
                {
                    await ProcessarMudancaPedido(change);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao processar mudança em pedido: {DocumentId}", 
                        change.DocumentKey["_id"]);
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro no monitoramento de pedidos");
            throw;
        }
    }

    /// <summary>
    /// Monitora mudanças na collection de clientes
    /// </summary>
    private async Task MonitorarClientes(CancellationToken cancellationToken)
    {
        try
        {
            var collection = _database.GetCollection<Cliente>("clientes");
            
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Cliente>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                               change.OperationType == ChangeStreamOperationType.Update ||
                               change.OperationType == ChangeStreamOperationType.Delete);

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };

            _logger.LogInformation("👥 Monitorando mudanças em CLIENTES...");

            using var cursor = await collection.WatchAsync(pipeline, options, cancellationToken);

            await cursor.ForEachAsync(async change =>
            {
                try
                {
                    await ProcessarMudancaCliente(change);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao processar mudança em cliente: {DocumentId}", 
                        change.DocumentKey["_id"]);
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro no monitoramento de clientes");
            throw;
        }
    }

    /// <summary>
    /// Monitora mudanças na collection de itens
    /// </summary>
    private async Task MonitorarItens(CancellationToken cancellationToken)
    {
        try
        {
            var collection = _database.GetCollection<Item>("itens");
            
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Item>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                               change.OperationType == ChangeStreamOperationType.Update ||
                               change.OperationType == ChangeStreamOperationType.Delete);

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };

            _logger.LogInformation("📦 Monitorando mudanças em ITENS...");

            using var cursor = await collection.WatchAsync(pipeline, options, cancellationToken);

            await cursor.ForEachAsync(async change =>
            {
                try
                {
                    await ProcessarMudancaItem(change);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao processar mudança em item: {DocumentId}", 
                        change.DocumentKey["_id"]);
                }
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro no monitoramento de itens");
            throw;
        }
    }

    /// <summary>
    /// Processa mudanças em pedidos
    /// </summary>
    private async Task ProcessarMudancaPedido(ChangeStreamDocument<Pedido> change)
    {
        var operacao = change.OperationType.ToString().ToLower();
        var documentId = change.DocumentKey["_id"].AsObjectId;

        _logger.LogInformation("📝 Processando {Operacao} em pedido: {DocumentId}", operacao, documentId);

        var evento = $"pedido_{operacao}";

        if (change.FullDocument != null)
        {
            // Para insert e update, temos o documento completo
            await _dataLakeService.SalvarEventoAsync(evento, operacao, "pedidos", documentId, change.FullDocument);
        }
        else
        {
            // Para delete, salvamos apenas os metadados
            await _dataLakeService.SalvarEventoAsync(evento, operacao, "pedidos", documentId, null);
        }
    }

    /// <summary>
    /// Processa mudanças em clientes
    /// </summary>
    private async Task ProcessarMudancaCliente(ChangeStreamDocument<Cliente> change)
    {
        var operacao = change.OperationType.ToString().ToLower();
        var documentId = change.DocumentKey["_id"].AsObjectId;

        _logger.LogInformation("👤 Processando {Operacao} em cliente: {DocumentId}", operacao, documentId);

        var evento = $"cliente_{operacao}";
        
        // Para clientes, salvamos apenas um evento simples (sem denormalização complexa)
        await _dataLakeService.SalvarEventoSimplesAsync(evento, operacao, "clientes", documentId);
    }

    /// <summary>
    /// Processa mudanças em itens
    /// </summary>
    private async Task ProcessarMudancaItem(ChangeStreamDocument<Item> change)
    {
        var operacao = change.OperationType.ToString().ToLower();
        var documentId = change.DocumentKey["_id"].AsObjectId;

        _logger.LogInformation("📦 Processando {Operacao} em item: {DocumentId}", operacao, documentId);

        var evento = $"item_{operacao}";
        
        // Para itens, salvamos apenas um evento simples
        await _dataLakeService.SalvarEventoSimplesAsync(evento, operacao, "itens", documentId);
    }
}