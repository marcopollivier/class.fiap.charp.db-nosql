using MongoDB.Driver;
using PedidosApi.Models;

namespace PedidosApi.Repositories;

public class MongoRepository
{
    private readonly IMongoDatabase _database;

    public MongoRepository(IMongoDatabase database)
    {
        _database = database;
    }

    // Operações de Cliente
    public async Task<string> CriarClienteAsync(Cliente cliente)
    {
        var collection = _database.GetCollection<Cliente>("clientes");
        await collection.InsertOneAsync(cliente);
        return cliente.Id;
    }

    public async Task<Cliente?> ObterClienteAsync(string id)
    {
        var collection = _database.GetCollection<Cliente>("clientes");
        return await collection.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Cliente>> ListarClientesAsync()
    {
        var collection = _database.GetCollection<Cliente>("clientes");
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task<bool> AtualizarClienteAsync(Cliente cliente)
    {
        var collection = _database.GetCollection<Cliente>("clientes");
        var result = await collection.ReplaceOneAsync(c => c.Id == cliente.Id, cliente);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarClienteAsync(string id)
    {
        var collection = _database.GetCollection<Cliente>("clientes");
        var result = await collection.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    // Operações de Pedido
    public async Task<string> CriarPedidoAsync(Pedido pedido)
    {
        var collection = _database.GetCollection<Pedido>("pedidos");
        await collection.InsertOneAsync(pedido);
        return pedido.Id;
    }

    public async Task<Pedido?> ObterPedidoAsync(string id)
    {
        var collection = _database.GetCollection<Pedido>("pedidos");
        return await collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Pedido>> ListarPedidosAsync()
    {
        var collection = _database.GetCollection<Pedido>("pedidos");
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task<List<Pedido>> ListarPedidosPorClienteAsync(string clienteId)
    {
        var collection = _database.GetCollection<Pedido>("pedidos");
        return await collection.Find(p => p.ClienteId == clienteId).ToListAsync();
    }
}
