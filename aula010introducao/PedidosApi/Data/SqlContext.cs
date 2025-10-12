using Microsoft.EntityFrameworkCore;
using PedidosApi.Models.Sql;

namespace PedidosApi.Data;

public class SqlContext : DbContext
{
    public SqlContext(DbContextOptions<SqlContext> options) : base(options) { }

    public DbSet<ClienteSql> Clientes { get; set; }
    public DbSet<PedidoSql> Pedidos { get; set; }
    public DbSet<ItemSql> Itens { get; set; }
}
