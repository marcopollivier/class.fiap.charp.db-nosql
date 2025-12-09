using Microsoft.EntityFrameworkCore;

using PedidosApi.Models.Sql;

namespace PedidosApi.Data;

public class SqlContext : DbContext
{
    public SqlContext(DbContextOptions<SqlContext> options) : base(options) { }

    public DbSet<ClienteSql> Clientes { get; set; } = null!;
    public DbSet<PedidoSql> Pedidos { get; set; } = null!;
    public DbSet<ItemSql> Itens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClienteSql>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<PedidoSql>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<ItemSql>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Preco).HasColumnType("decimal(18,2)");
        });

        base.OnModelCreating(modelBuilder);
    }
}
