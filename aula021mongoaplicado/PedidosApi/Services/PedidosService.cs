using MongoDB.Bson;
using PedidosApi.Models;
using PedidosApi.Repositories;

namespace PedidosApi.Services;

public class PedidosService
{
    private readonly MongoRepository _mongoRepository;
    private readonly ILogger<PedidosService> _logger;

    public PedidosService(MongoRepository mongoRepository, ILogger<PedidosService> logger)
    {
        _mongoRepository = mongoRepository;
        _logger = logger;
    }

    // Operações de Cliente
    public async Task<string> CriarClienteAsync(ClienteDto clienteDto)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de cliente: {Nome}", clienteDto.Nome);

            var cliente = new Cliente
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Nome = clienteDto.Nome,
                Email = clienteDto.Email
            };

            var id = await _mongoRepository.CriarClienteAsync(cliente);
            _logger.LogInformation("Cliente criado com ID: {Id}", id);

            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar cliente");
            throw;
        }
    }

    public async Task<Cliente?> BuscarClienteAsync(string id)
    {
        try
        {
            _logger.LogInformation("Buscando cliente com ID: {Id}", id);
            return await _mongoRepository.ObterClienteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cliente");
            throw;
        }
    }

    public async Task<List<Cliente>> ListarClientesAsync()
    {
        try
        {
            _logger.LogInformation("Listando todos os clientes");
            return await _mongoRepository.ListarClientesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar clientes");
            throw;
        }
    }

    public async Task<bool> AtualizarClienteAsync(string id, ClienteDto clienteDto)
    {
        try
        {
            _logger.LogInformation("Atualizando cliente com ID: {Id}", id);

            // Verificar se o cliente existe
            var clienteExistente = await _mongoRepository.ObterClienteAsync(id);
            if (clienteExistente == null)
            {
                _logger.LogWarning("Cliente com ID {Id} não encontrado para atualização", id);
                return false;
            }

            // Atualizar os dados
            clienteExistente.Nome = clienteDto.Nome;
            clienteExistente.Email = clienteDto.Email;

            var sucesso = await _mongoRepository.AtualizarClienteAsync(clienteExistente);

            if (sucesso)
            {
                _logger.LogInformation("Cliente com ID {Id} atualizado com sucesso", id);
            }
            else
            {
                _logger.LogWarning("Falha ao atualizar cliente com ID {Id}", id);
            }

            return sucesso;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cliente com ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeletarClienteAsync(string id)
    {
        try
        {
            _logger.LogInformation("Deletando cliente com ID: {Id}", id);

            // Verificar se o cliente possui pedidos
            var pedidosDoCliente = await _mongoRepository.ListarPedidosPorClienteAsync(id);
            if (pedidosDoCliente.Any())
            {
                _logger.LogWarning("Tentativa de deletar cliente com ID {Id} que possui {QuantidadePedidos} pedidos", id, pedidosDoCliente.Count);
                throw new InvalidOperationException("Não é possível deletar cliente que possui pedidos. Delete os pedidos primeiro.");
            }

            var sucesso = await _mongoRepository.DeletarClienteAsync(id);

            if (sucesso)
            {
                _logger.LogInformation("Cliente com ID {Id} deletado com sucesso", id);
            }
            else
            {
                _logger.LogWarning("Cliente com ID {Id} não foi encontrado para deleção", id);
            }

            return sucesso;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar cliente com ID: {Id}", id);
            throw;
        }
    }

    // Operações de Pedido
    public async Task<string> CriarPedidoAsync(PedidoDto pedidoDto)
    {
        try
        {
            _logger.LogInformation("Iniciando criação de pedido para cliente: {ClienteId}", pedidoDto.ClienteId);

            // Verificar se o cliente existe
            var cliente = await _mongoRepository.ObterClienteAsync(pedidoDto.ClienteId);
            if (cliente == null)
            {
                throw new ArgumentException("Cliente não encontrado", nameof(pedidoDto.ClienteId));
            }

            var pedido = new Pedido
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ClienteId = pedidoDto.ClienteId,
                DataPedido = DateTime.UtcNow,
                Itens = pedidoDto.Itens.Select(i => new Item
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            var id = await _mongoRepository.CriarPedidoAsync(pedido);
            _logger.LogInformation("Pedido criado com ID: {Id}", id);

            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            throw;
        }
    }

    public async Task<Pedido?> BuscarPedidoAsync(string id)
    {
        try
        {
            _logger.LogInformation("Buscando pedido com ID: {Id}", id);
            return await _mongoRepository.ObterPedidoAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido");
            throw;
        }
    }

    public async Task<List<Pedido>> ListarPedidosAsync()
    {
        try
        {
            _logger.LogInformation("Listando todos os pedidos");
            return await _mongoRepository.ListarPedidosAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos");
            throw;
        }
    }

    public async Task<List<Pedido>> ListarPedidosPorClienteAsync(string clienteId)
    {
        try
        {
            _logger.LogInformation("Listando pedidos do cliente: {ClienteId}", clienteId);
            return await _mongoRepository.ListarPedidosPorClienteAsync(clienteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos do cliente");
            throw;
        }
    }
}
