using PedidosApi.Repositories;
using PedidosApi.Services;

namespace PedidosApi.Extensions;

/// <summary>
/// Extensões para configuração dos serviços da API
/// </summary>
public static class ApiServiceExtensions
{
    /// <summary>
    /// Adiciona os serviços da aplicação
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Registrar repositórios e serviços
        services.AddScoped<MongoRepository>();
        services.AddScoped<PedidosService>();

        // Configurar controllers
        services.AddControllers();

        return services;
    }

    /// <summary>
    /// Configura o pipeline da aplicação
    /// </summary>
    /// <param name="app">Aplicação web</param>
    /// <returns>Aplicação configurada</returns>
    public static WebApplication ConfigureApplicationPipeline(this WebApplication app)
    {
        // Configurar Swagger em desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedidos API v1");
                options.RoutePrefix = string.Empty; // Swagger na raiz
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
