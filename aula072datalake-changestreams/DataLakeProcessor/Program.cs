using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using DataLakeProcessor.Services;

namespace DataLakeProcessor;

/// <summary>
/// Aplicação principal para processar Change Streams e alimentar o Data Lake
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 Data Lake Processor - Change Streams MongoDB");
        Console.WriteLine("================================================");
        
        try
        {
            // Configurar Host
            var host = CreateHostBuilder(args).Build();
            
            // Validar configuração
            await ValidarConfiguracaoAsync(host);
            
            // Iniciar processamento
            await ExecutarProcessamentoAsync(host);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro fatal: {ex.Message}");
            Console.WriteLine($"🔍 Detalhes: {ex}");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Configura o Host Builder com DI e configurações
    /// </summary>
    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;
                
                // MongoDB
                var connectionString = configuration.GetConnectionString("MongoDB") 
                    ?? configuration.GetSection("MongoDB:ConnectionString").Value
                    ?? throw new InvalidOperationException("MongoDB connection string não configurada");
                
                var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value
                    ?? throw new InvalidOperationException("MongoDB database name não configurado");
                
                services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
                services.AddSingleton<IMongoDatabase>(sp =>
                {
                    var client = sp.GetRequiredService<IMongoClient>();
                    return client.GetDatabase(databaseName);
                });
                
                // Serviços da aplicação
                services.AddSingleton<DataLakeService>();
                services.AddSingleton<ChangeStreamProcessor>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

    /// <summary>
    /// Valida se a configuração e conexões estão funcionais
    /// </summary>
    static async Task ValidarConfiguracaoAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

        logger.LogInformation("🔍 Validando configurações...");

        // Testar conexão com MongoDB
        try
        {
            var command = new MongoDB.Bson.BsonDocument("ping", 1);
            await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(command);
            logger.LogInformation("✅ Conexão com MongoDB: OK");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Falha na conexão com MongoDB");
            throw new InvalidOperationException("Não foi possível conectar ao MongoDB. Verifique se o Replica Set está configurado.", ex);
        }

        // Verificar se é Replica Set
        try
        {
            var statusCommand = new MongoDB.Bson.BsonDocument("isMaster", 1);
            var result = await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(statusCommand);
            
            if (!result.Contains("setName"))
            {
                throw new InvalidOperationException("MongoDB deve estar configurado como Replica Set para usar Change Streams");
            }
            
            logger.LogInformation("✅ Replica Set configurado: {SetName}", result["setName"]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Problema com configuração do Replica Set");
            throw;
        }

        // Verificar collections
        var collectionNames = await (await database.ListCollectionNamesAsync()).ToListAsync();
        var requiredCollections = new[] { "clientes", "pedidos", "itens" };
        
        foreach (var collection in requiredCollections)
        {
            if (collectionNames.Contains(collection))
            {
                logger.LogInformation("✅ Collection {Collection}: OK", collection);
            }
            else
            {
                logger.LogWarning("⚠️ Collection {Collection}: Não encontrada (será criada quando houver dados)", collection);
            }
        }

        logger.LogInformation("✅ Validação concluída com sucesso!");
    }

    /// <summary>
    /// Executa o processamento principal da aplicação
    /// </summary>
    static async Task ExecutarProcessamentoAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var processor = scope.ServiceProvider.GetRequiredService<ChangeStreamProcessor>();
        var dataLakeService = scope.ServiceProvider.GetRequiredService<DataLakeService>();

        // Configurar cancellation token
        using var cts = new CancellationTokenSource();
        
        // Capturar Ctrl+C para parada elegante
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            logger.LogInformation("🛑 Solicitação de parada recebida...");
            cts.Cancel();
        };

        logger.LogInformation("🎯 Iniciando processamento do Data Lake...");
        logger.LogInformation("💡 Pressione Ctrl+C para parar graciosamente");
        logger.LogInformation("");

        // Mostrar estatísticas iniciais
        try
        {
            var stats = await dataLakeService.ObterEstatisticasAsync();
            logger.LogInformation("📊 Estatísticas atuais do Data Lake:");
            logger.LogInformation("   {Stats}", System.Text.Json.JsonSerializer.Serialize(stats, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
            logger.LogInformation("");
        }
        catch (Exception ex)
        {
            logger.LogWarning("⚠️ Não foi possível obter estatísticas iniciais: {Error}", ex.Message);
        }

        try
        {
            // Iniciar processamento
            await processor.StartAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("✅ Processamento parado pelo usuário");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Erro durante o processamento");
            throw;
        }
        finally
        {
            // Mostrar estatísticas finais
            try
            {
                var finalStats = await dataLakeService.ObterEstatisticasAsync();
                logger.LogInformation("");
                logger.LogInformation("📊 Estatísticas finais do Data Lake:");
                logger.LogInformation("   {Stats}", System.Text.Json.JsonSerializer.Serialize(finalStats, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
            }
            catch (Exception ex)
            {
                logger.LogWarning("⚠️ Não foi possível obter estatísticas finais: {Error}", ex.Message);
            }
            
            logger.LogInformation("👋 Data Lake Processor finalizado");
        }
    }
}