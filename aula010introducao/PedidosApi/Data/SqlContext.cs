using Microsoft.EntityFrameworkCore;

using PedidosApi.Models.Sql;

namespace PedidosApi.Data;

public class SqlContext : DbContext
{
    public SqlContext(DbContextOptions<SqlContext> options) : base(options) { }

    public DbSet<ClienteSql> Clientes { get; set; } = null!;
    public DbSet<PedidoSql> Pedidos { get; set; } = null!;
    public DbSet<ItemSql> Itens { get; set; } = null!;
}
