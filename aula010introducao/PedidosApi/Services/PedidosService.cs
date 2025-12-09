using PedidosApi.Models;
using PedidosApi.Repositories;

namespace PedidosApi.Services;

public class PedidosService
{
    private readonly MongoRepository _mongoRepository;
    private readonly SqlRepository _sqlRepository;
    private readonly ILogger<PedidosService> _logger;

    public PedidosService(MongoRepository mongoRepository, SqlRepository sqlRepository, ILogger<PedidosService> logger)
    {
        _mongoRepository = mongoRepository;
        _sqlRepository = sqlRepository;
        _logger = logger;
    }

    public async Task<object> CriarClienteAsync(ClienteDto clienteDto)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de cliente: {Nome}", clienteDto.Nome);

            // Criar no MongoDB
            var clienteMongo = new Cliente
            {
                Nome = clienteDto.Nome,
                Email = clienteDto.Email
            };
            var mongoId = await _mongoRepository.CriarClienteAsync(clienteMongo);

            // Criar no SQL Server
            var clienteSql = new Models.Sql.ClienteSql
            {
                Nome = clienteDto.Nome,
                Email = clienteDto.Email
            };
            var sqlId = await _sqlRepository.CriarClienteAsync(clienteSql);

            _logger.LogInformation("Cliente criado - MongoDB ID: {MongoId}, SQL Server ID: {SqlId}", mongoId, sqlId);

            return new
            {
                MongoId = mongoId,
                SqlId = sqlId,
                Message = "Cliente criado em ambos os bancos de dados"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            throw;
        }
    }

    public async Task<object> BuscarClienteAsync(string mongoId, int sqlId)
    {
        try
        {
            _logger.LogInformation("Buscando cliente - MongoDB ID: {MongoId}, SQL ID: {SqlId}", mongoId, sqlId);

            // Buscar no MongoDB
            var clienteMongo = await _mongoRepository.ObterClienteAsync(mongoId);

            // Buscar no SQL Server
            var clienteSql = await _sqlRepository.ObterClienteAsync(sqlId);

            return new
            {
                MongoDB = clienteMongo,
                SqlServer = clienteSql,
                Comparacao = new
                {
                    MongoIdType = "ObjectId (string)",
                    SqlIdType = "int (auto-increment)",
                    MongoFlexibilidade = "Schema flexível",
                    SqlEstrutura = "Schema rígido"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cliente");
            throw;
        }
    }

    public async Task<object> CriarPedidoAsync(PedidoDto pedidoDto)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de pedido para cliente: {ClienteId}", pedidoDto.ClienteId);

            // Criar no MongoDB (documento embarcado)
            var pedidoMongo = new Pedido
            {
                ClienteId = pedidoDto.ClienteId,
                DataPedido = DateTime.UtcNow,
                Itens = pedidoDto.Itens.Select(i => new Item
                {
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Quantidade = i.Quantidade
                }).ToList()
            };
            var mongoId = await _mongoRepository.CriarPedidoAsync(pedidoMongo);

            // Criar no SQL Server (tabelas relacionadas)
            var pedidoSql = new Models.Sql.PedidoSql
            {
                ClienteId = pedidoDto.SqlClienteId,
                DataPedido = DateTime.UtcNow,
                Itens = pedidoDto.Itens.Select(i => new Models.Sql.ItemSql
                {
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Quantidade = i.Quantidade
                }).ToList()
            };
            var sqlId = await _sqlRepository.CriarPedidoAsync(pedidoSql);

            _logger.LogInformation("Pedido criado - MongoDB ID: {MongoId}, SQL Server ID: {SqlId}", mongoId, sqlId);

            return new
            {
                MongoId = mongoId,
                SqlId = sqlId,
                Message = "Pedido criado em ambos os bancos de dados",
                ModelagemComparacao = new
                {
                    MongoDB = "Documento embarcado - pedido contém itens",
                    SqlServer = "Tabelas relacionadas - chaves estrangeiras"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            throw;
        }
    }

    public async Task<object> BuscarPedidoAsync(string mongoId, int sqlId)
    {
        try
        {
            _logger.LogInformation("Buscando pedido - MongoDB ID: {MongoId}, SQL ID: {SqlId}", mongoId, sqlId);

            // Buscar no MongoDB
            var pedidoMongo = await _mongoRepository.ObterPedidoAsync(mongoId);

            // Buscar no SQL Server
            var pedidoSql = await _sqlRepository.ObterPedidoAsync(sqlId);

            return new
            {
                MongoDB = pedidoMongo,
                SqlServer = pedidoSql,
                ModelagemComparacao = new
                {
                    MongoConsulta = "Uma única consulta - documento completo",
                    SqlConsulta = "Múltiplas consultas ou JOINs necessários",
                    MongoVantagem = "Menos roundtrips ao banco",
                    SqlVantagem = "Normalização evita duplicação"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido");
            throw;
        }
    }
}
