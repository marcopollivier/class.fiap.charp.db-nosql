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

            var id = Guid.NewGuid().ToString();

            var clienteMongo = new Cliente
            {
                Id = id,
                Nome = clienteDto.Nome,
                Email = clienteDto.Email
            };
            await _mongoRepository.CriarClienteAsync(clienteMongo);

            var clienteSql = new Models.Sql.ClienteSql
            {
                Id = id,
                Nome = clienteDto.Nome,
                Email = clienteDto.Email
            };
            await _sqlRepository.CriarClienteAsync(clienteSql);

            _logger.LogInformation("Cliente criado com ID: {Id}", id);

            return new
            {
                Id = id,
                Message = "Cliente criado em ambos os bancos de dados com o mesmo ID"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            throw;
        }
    }

    public async Task<object> BuscarClienteAsync(string id)
    {
        try
        {
            _logger.LogInformation("Buscando cliente com ID: {Id}", id);

            var clienteMongo = await _mongoRepository.ObterClienteAsync(id);
            var clienteSql = await _sqlRepository.ObterClienteAsync(id);

            return new
            {
                Id = id,
                MongoDB = clienteMongo,
                SqlServer = clienteSql,
                Comparacao = new
                {
                    IdUnico = "GUID usado em ambos os bancos",
                    MongoVantagem = "Flexibilidade de schema",
                    SqlVantagem = "Estrutura rígida e consistente"
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

            var id = Guid.NewGuid().ToString();

            var pedidoMongo = new Pedido
            {
                Id = id,
                ClienteId = pedidoDto.ClienteId,
                DataPedido = DateTime.UtcNow,
                Itens = pedidoDto.Itens.Select(i => new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Quantidade = i.Quantidade
                }).ToList()
            };
            await _mongoRepository.CriarPedidoAsync(pedidoMongo);

            var pedidoSql = new Models.Sql.PedidoSql
            {
                Id = id,
                ClienteId = pedidoDto.ClienteId,
                DataPedido = DateTime.UtcNow,
                Itens = pedidoDto.Itens.Select(i => new Models.Sql.ItemSql
                {
                    Id = Guid.NewGuid().ToString(),
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Quantidade = i.Quantidade
                }).ToList()
            };
            await _sqlRepository.CriarPedidoAsync(pedidoSql);

            _logger.LogInformation("Pedido criado com ID: {Id}", id);

            return new
            {
                Id = id,
                Message = "Pedido criado em ambos os bancos de dados com o mesmo ID",
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

    public async Task<object> BuscarPedidoAsync(string id)
    {
        try
        {
            _logger.LogInformation("Buscando pedido com ID: {Id}", id);

            var pedidoMongo = await _mongoRepository.ObterPedidoAsync(id);
            var pedidoSql = await _sqlRepository.ObterPedidoAsync(id);

            return new
            {
                Id = id,
                MongoDB = pedidoMongo,
                SqlServer = pedidoSql,
                ModelagemComparacao = new
                {
                    IdUnico = "Mesmo GUID em ambos os bancos",
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
