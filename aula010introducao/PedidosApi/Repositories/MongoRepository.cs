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

    public async Task<string> CriarClienteAsync(Cliente cliente)
    {
        var collection = _database.GetCollection<Cliente>("clientes");
        await collection.InsertOneAsync(cliente);
        return cliente.Id!;
    }

    public async Task<Cliente?> ObterClienteAsync(string id)
    {
        var collection = _database.GetCollection<Cliente>("clientes");
        return await collection.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<string> CriarPedidoAsync(Pedido pedido)
    {
        var collection = _database.GetCollection<Pedido>("pedidos");
        await collection.InsertOneAsync(pedido);
        return pedido.Id!;
    }

    public async Task<Pedido?> ObterPedidoAsync(string id)
    {
        var collection = _database.GetCollection<Pedido>("pedidos");
        return await collection.Find(p => p.Id == id).FirstOrDefaultAsync();
    }
}
