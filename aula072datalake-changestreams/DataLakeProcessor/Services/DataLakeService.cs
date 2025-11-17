using MongoDB.Driver;
using MongoDB.Bson;
using DataLakeProcessor.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DataLakeProcessor.Services;

/// <summary>
/// Serviço responsável por salvar eventos no Data Lake
/// </summary>
public class DataLakeService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<DataLakeService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMongoCollection<EventoDataLake> _dataLakeCollection;

    public DataLakeService(
        IMongoDatabase database,
        ILogger<DataLakeService> logger,
        IConfiguration configuration)
    {
        _database = database;
        _logger = logger;
        _configuration = configuration;
        
        var collectionName = configuration.GetSection("DataLake:CollectionName").Value ?? "datalake";
        _dataLakeCollection = database.GetCollection<EventoDataLake>(collectionName);
    }

    /// <summary>
    /// Salva evento completo com denormalização de pedido
    /// </summary>
    public async Task SalvarEventoAsync(string evento, string operacao, string colecao, ObjectId documentId, Pedido? pedido)
    {
        try
        {
            var eventoDataLake = new EventoDataLake
            {
                Id = ObjectId.GenerateNewId(),
                Evento = evento,
                Timestamp = DateTime.UtcNow,
                Operacao = operacao,
                Colecao = colecao,
                DocumentId = documentId,
                Metadados = new MetadadosEvento
                {
                    Origem = "change_stream",
                    Versao = _configuration.GetSection("DataLake:ProcessorVersion").Value ?? "1.0",
                    ProcessadoEm = DateTime.UtcNow,
                    Hostname = Environment.MachineName
                }
            };

            // Se temos um pedido, denormalizar os dados
            if (pedido != null)
            {
                eventoDataLake.Pedido = await DenormalizarPedidoAsync(pedido);
            }

            await _dataLakeCollection.InsertOneAsync(eventoDataLake);
            
            _logger.LogInformation("✅ Evento salvo no Data Lake: {Evento} - {DocumentId}", evento, documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao salvar evento no Data Lake: {Evento} - {DocumentId}", evento, documentId);
            throw;
        }
    }

    /// <summary>
    /// Salva evento simples sem denormalização
    /// </summary>
    public async Task SalvarEventoSimplesAsync(string evento, string operacao, string colecao, ObjectId documentId)
    {
        try
        {
            var eventoDataLake = new EventoDataLake
            {
                Id = ObjectId.GenerateNewId(),
                Evento = evento,
                Timestamp = DateTime.UtcNow,
                Operacao = operacao,
                Colecao = colecao,
                DocumentId = documentId,
                Metadados = new MetadadosEvento
                {
                    Origem = "change_stream",
                    Versao = _configuration.GetSection("DataLake:ProcessorVersion").Value ?? "1.0",
                    ProcessadoEm = DateTime.UtcNow,
                    Hostname = Environment.MachineName
                }
            };

            await _dataLakeCollection.InsertOneAsync(eventoDataLake);
            
            _logger.LogInformation("✅ Evento simples salvo no Data Lake: {Evento} - {DocumentId}", evento, documentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao salvar evento simples no Data Lake: {Evento} - {DocumentId}", evento, documentId);
            throw;
        }
    }

    /// <summary>
    /// Denormaliza um pedido buscando dados relacionados do cliente
    /// </summary>
    private async Task<PedidoDenormalizado> DenormalizarPedidoAsync(Pedido pedido)
    {
        // Buscar dados do cliente
        var clienteCollection = _database.GetCollection<Cliente>("clientes");
        var cliente = await clienteCollection.Find(c => c.Id == pedido.ClienteId).FirstOrDefaultAsync();

        var clienteResumido = new ClienteResumido();
        if (cliente != null)
        {
            clienteResumido = new ClienteResumido
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email
            };
        }
        else
        {
            _logger.LogWarning("⚠️ Cliente não encontrado: {ClienteId}", pedido.ClienteId);
            clienteResumido = new ClienteResumido
            {
                Id = pedido.ClienteId,
                Nome = "Cliente não encontrado",
                Email = "N/A"
            };
        }

        return new PedidoDenormalizado
        {
            Id = pedido.Id,
            Total = pedido.Total,
            DataPedido = pedido.DataPedido,
            Cliente = clienteResumido,
            Itens = pedido.Itens
        };
    }

    /// <summary>
    /// Obtém estatísticas do Data Lake
    /// </summary>
    public async Task<object> ObterEstatisticasAsync()
    {
        try
        {
            var totalEventos = await _dataLakeCollection.CountDocumentsAsync(FilterDefinition<EventoDataLake>.Empty);
            
            var eventosPorTipo = await _dataLakeCollection.Aggregate()
                .Group(e => e.Evento, g => new { Evento = g.Key, Count = g.Count() })
                .ToListAsync();

            var eventosPorOperacao = await _dataLakeCollection.Aggregate()
                .Group(e => e.Operacao, g => new { Operacao = g.Key, Count = g.Count() })
                .ToListAsync();

            var ultimosEventos = await _dataLakeCollection.Find(FilterDefinition<EventoDataLake>.Empty)
                .SortByDescending(e => e.Timestamp)
                .Limit(5)
                .Project(e => new { e.Evento, e.Timestamp, e.DocumentId })
                .ToListAsync();

            return new
            {
                TotalEventos = totalEventos,
                EventosPorTipo = eventosPorTipo,
                EventosPorOperacao = eventosPorOperacao,
                UltimosEventos = ultimosEventos
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao obter estatísticas do Data Lake");
            throw;
        }
    }
}