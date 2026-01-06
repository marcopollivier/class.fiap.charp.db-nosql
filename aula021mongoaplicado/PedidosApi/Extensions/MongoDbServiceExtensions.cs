using MongoDB.Driver;

namespace PedidosApi.Extensions;

/// <summary>
/// Extensões para configuração dos serviços do MongoDB
/// </summary>
public static class MongoDbServiceExtensions
{
    /// <summary>
    /// Adiciona e configura os serviços do MongoDB
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>Coleção de serviços configurada</returns>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB")
            ?? throw new InvalidOperationException("MongoDB connection string not found");

        var databaseName = configuration["DatabaseSettings:MongoDbDatabaseName"]
            ?? throw new InvalidOperationException("MongoDB database name not found");

        // Registrar cliente MongoDB como singleton
        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(connectionString);
        });

        // Registrar database MongoDB como singleton
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        return services;
    }
}
