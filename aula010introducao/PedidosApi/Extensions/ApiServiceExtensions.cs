using PedidosApi.Repositories;
using PedidosApi.Services;

namespace PedidosApi.Extensions;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<MongoRepository>();
        services.AddScoped<SqlRepository>();
        services.AddScoped<PedidosService>();
        services.AddControllers();

        return services;
    }

    public static WebApplication ConfigureApplicationPipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
