using PedidosApi.Repositories;
using PedidosApi.Services;

namespace PedidosApi.Extensions;

/// <summary>
/// Extensões para configuração dos serviços da API
/// </summary>
public static class ApiServiceExtensions
{
    /// <summary>
    /// Adiciona e configura os serviços básicos da API
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Registrar repositórios
        services.AddScoped<MongoRepository>();
        services.AddScoped<SqlRepository>();

        // Registrar service unificado
        services.AddScoped<PedidosService>();

        // Serviços básicos da API
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        return services;
    }

    /// <summary>
    /// Adiciona e configura o Swagger para documentação da API
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "Pedidos API",
                Version = "v1",
                Description = "API para demonstração de NoSQL vs SQL com .NET"
            });
        });

        return services;
    }

    /// <summary>
    /// Configura o pipeline de desenvolvimento da aplicação
    /// </summary>
    /// <param name="app">Aplicação web</param>
    /// <returns>Aplicação web configurada</returns>
    public static WebApplication ConfigureDevelopmentPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedidos API v1");
                c.RoutePrefix = string.Empty; // Swagger na raiz
            });
        }

        return app;
    }

    /// <summary>
    /// Configura o pipeline da aplicação
    /// </summary>
    /// <param name="app">Aplicação web</param>
    /// <returns>Aplicação web configurada</returns>
    public static WebApplication ConfigureApplicationPipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
