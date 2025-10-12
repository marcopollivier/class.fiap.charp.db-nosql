using Microsoft.EntityFrameworkCore;

using PedidosApi.Data;
using PedidosApi.Models.Sql;

namespace PedidosApi.Repositories;

public class SqlRepository
{
    private readonly SqlContext _context;

    public SqlRepository(SqlContext context)
    {
        _context = context;
    }

    public async Task<int> CriarClienteAsync(ClienteSql cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente.Id;
    }

    public async Task<ClienteSql?> ObterClienteAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<int> CriarPedidoAsync(PedidoSql pedido)
    {
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        foreach (var item in pedido.Itens)
        {
            item.PedidoId = pedido.Id;
            _context.Itens.Add(item);
        }
        await _context.SaveChangesAsync();

        return pedido.Id;
    }

    public async Task<PedidoSql?> ObterPedidoAsync(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido != null)
        {
            pedido.Itens = await _context.Itens.Where(i => i.PedidoId == id).ToListAsync();
        }
        return pedido;
    }
}
