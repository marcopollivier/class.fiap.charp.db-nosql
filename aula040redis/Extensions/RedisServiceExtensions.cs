using StackExchange.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PedidosApi.Extensions;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuração do Redis
        var connectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        var password = configuration["Redis:Password"] ?? "password123";

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configOptions = ConfigurationOptions.Parse(connectionString);
            configOptions.Password = password;
            configOptions.AbortOnConnectFail = false;
            configOptions.ConnectRetry = 3;
            configOptions.ConnectTimeout = 5000;
            configOptions.SyncTimeout = 5000;
            configOptions.AsyncTimeout = 5000;

            return ConnectionMultiplexer.Connect(configOptions);
        });

        // Registrar IDatabase para injeção
        services.AddScoped<IDatabase>(provider =>
            provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

        return services;
    }
}

// Exemplo de Health Check para Redis
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public RedisHealthCheck(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = _redis.GetDatabase();
            await database.PingAsync();

            return HealthCheckResult.Healthy("Redis is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unhealthy", ex);
        }
    }
}
