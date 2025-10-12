using Microsoft.EntityFrameworkCore;
using PedidosApi.Data;

namespace PedidosApi.Extensions;

/// <summary>
/// Extensões para configuração dos serviços do SQL Server
/// </summary>
public static class SqlServerServiceExtensions
{
    /// <summary>
    /// Adiciona e configura os serviços do SQL Server
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddSqlServer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("SQL Server connection string not found");

        services.AddDbContext<SqlContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }

    /// <summary>
    /// Inicializa o banco de dados SQL Server
    /// </summary>
    /// <param name="app">Aplicação web</param>
    /// <returns>Aplicação web configurada</returns>
    public static WebApplication InitializeSqlServerDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
        context.Database.EnsureCreated();

        return app;
    }
}