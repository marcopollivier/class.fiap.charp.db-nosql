using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ClientApp.DynamoDemo.Models;
using ClientApp.DynamoDemo.Services;

namespace ClientApp.DynamoDemo;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices)
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var dynamoService = host.Services.GetRequiredService<IDynamoDbService>();

        try
        {
            logger.LogInformation("🚀 Iniciando demonstração DynamoDB com Modelos Tipados - FIAP NoSQL");

            var demo = new DynamoDemoService(dynamoService, logger);
            await demo.ExecutarDemonstracaoCompletaAsync();

            logger.LogInformation("✅ Demonstração concluída com sucesso!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Erro durante a execução da demonstração");
            Environment.Exit(1);
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAmazonDynamoDB>(provider => new AmazonDynamoDBClient());
        services.AddSingleton<IDynamoDBContext>(provider =>
        {
            var client = provider.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDBContext(client);
        });

        services.AddScoped<IDynamoDbService, DynamoDbService>();
        services.AddLogging(builder =>
            builder.AddConsole().SetMinimumLevel(LogLevel.Information));
    }
}

public class DynamoDemoService
{
    private readonly IDynamoDbService _dynamoService;
    private readonly ILogger _logger;

    public DynamoDemoService(IDynamoDbService dynamoService, ILogger logger)
    {
        _dynamoService = dynamoService;
        _logger = logger;
    }

    public async Task ExecutarDemonstracaoCompletaAsync()
    {
        _logger.LogInformation("📚 === AULA 5: DynamoDB com Modelos Tipados - Tabela PedidosApp na AWS ===");

        // 1. Verificar se tabela existe na AWS (não criar, apenas validar)
        await ValidarTabelaExistenteAsync();

        // 2. Demonstrar operações CRUD com modelos tipados
        await DemonstrarCrudClienteAsync();
        await DemonstrarCrudProfileAsync();
        await DemonstrarCrudPedidoAsync();
        await DemonstrarCrudItemAsync();
        await DemonstrarCrudEnderecoAsync();

        // 3. Consultas complexas e relatórios
        await DemonstrarConsultasComplexasAsync();

        // 4. Estatísticas e métricas
        await DemonstrarEstatisticasAsync();
    }

    private async Task ValidarTabelaExistenteAsync()
    {
        _logger.LogInformation("🔧 Validando tabela PedidosApp na AWS...");

        var existe = await _dynamoService.VerificarTabelaExistenteAsync();
        if (existe)
        {
            _logger.LogInformation("✅ Tabela PedidosApp encontrada e ativa!");
        }
        else
        {
            _logger.LogError("❌ Tabela PedidosApp não encontrada. Verifique se foi criada corretamente.");
            throw new InvalidOperationException("Tabela não encontrada");
        }
    }

    private async Task DemonstrarCrudClienteAsync()
    {
        _logger.LogInformation("👤 === CRUD Cliente com Modelo Tipado ===");

        // CREATE
        var cliente = new Cliente(789, "Ana Costa", "ana.costa@email.com");
        await _dynamoService.InserirClienteAsync(cliente);

        // READ
        var clienteLido = await _dynamoService.ObterClienteAsync(789);
        if (clienteLido != null)
        {
            _logger.LogInformation("📖 Cliente lido: {Cliente}", clienteLido);
        }

        // UPDATE
        if (clienteLido != null)
        {
            clienteLido.Email = "ana.costa.novo@email.com";
            await _dynamoService.AtualizarClienteAsync(clienteLido);
        }

        // LIST
        var todosClientes = await _dynamoService.ListarTodosClientesAsync();
        _logger.LogInformation("📋 Total de clientes: {Count}", todosClientes.Count);
    }

    private async Task DemonstrarCrudProfileAsync()
    {
        _logger.LogInformation("📱 === CRUD Profile com Modelo Tipado ===");

        var profile = new Profile(789, "+55 11 98765-4321", new DateTime(1990, 3, 15));
        profile.AdicionarPreferencia("email");
        profile.AdicionarPreferencia("sms");
        profile.Profissao = "Desenvolvedora";
        profile.AceitaMarketing = true;

        await _dynamoService.InserirProfileAsync(profile);

        var profileLido = await _dynamoService.ObterProfileAsync(789);
        if (profileLido != null)
        {
            _logger.LogInformation("📖 Profile lido: {Profile}", profileLido);
            _logger.LogInformation("🎂 Idade calculada: {Idade} anos", profileLido.Idade);
        }
    }

    private async Task DemonstrarCrudPedidoAsync()
    {
        _logger.LogInformation("🛒 === CRUD Pedido com Modelo Tipado ===");

        var pedido = new Pedido("2025-0010", 789, 299.90m)
        {
            Status = StatusPedido.CONFIRMADO,
            Observacoes = "Entrega urgente"
        };

        await _dynamoService.InserirPedidoAsync(pedido);

        // Listar pedidos do cliente
        var pedidos = await _dynamoService.ListarPedidosClienteAsync(789);
        _logger.LogInformation("📋 Cliente tem {Count} pedidos", pedidos.Count);

        foreach (var p in pedidos)
        {
            _logger.LogInformation("🛍️ {Pedido}", p);
        }
    }

    private async Task DemonstrarCrudItemAsync()
    {
        _logger.LogInformation("📦 === CRUD Item com Modelo Tipado ===");

        var item1 = new Item(789, "2025-0010", 1, "Notebook Gamer", 1, 2499.90m)
        {
            CodigoProduto = "NB-001",
            Observacoes = "Cor: Preto"
        };

        var item2 = new Item(789, "2025-0010", 2, "Mouse Gamer", 1, 199.90m)
        {
            CodigoProduto = "MS-002"
        };

        await _dynamoService.InserirItemAsync(item1);
        await _dynamoService.InserirItemAsync(item2);

        // Listar itens do pedido
        var itens = await _dynamoService.ListarItensDoPedidoAsync(789, "2025-0010");
        _logger.LogInformation("📋 Pedido tem {Count} itens", itens.Count);

        decimal totalCalculado = 0;
        foreach (var item in itens)
        {
            _logger.LogInformation("📦 {Item}", item);
            totalCalculado += item.ValorTotal;
        }

        _logger.LogInformation("💰 Total calculado dos itens: R${Total:F2}", totalCalculado);
    }

    private async Task DemonstrarCrudEnderecoAsync()
    {
        _logger.LogInformation("🏠 === CRUD Endereço com Modelo Tipado ===");

        var enderecoHome = new Endereco(789, TipoEndereco.HOME, "Rua das Flores", "123",
            "Centro", "São Paulo", "SP", "01234-567")
        {
            Nome = "Casa",
            Complemento = "Apto 45",
            Principal = true,
            Observacoes = "Portão azul"
        };

        var enderecoWork = new Endereco(789, TipoEndereco.WORK, "Av. Paulista", "1000",
            "Bela Vista", "São Paulo", "SP", "01310-100")
        {
            Nome = "Escritório",
            Complemento = "15º andar"
        };

        await _dynamoService.InserirEnderecoAsync(enderecoHome);
        await _dynamoService.InserirEnderecoAsync(enderecoWork);

        // Listar endereços
        var enderecos = await _dynamoService.ListarEnderecosClienteAsync(789);
        _logger.LogInformation("📋 Cliente tem {Count} endereços", enderecos.Count);

        foreach (var endereco in enderecos)
        {
            _logger.LogInformation("🏠 {Endereco}", endereco);
        }
    }

    private async Task DemonstrarConsultasComplexasAsync()
    {
        _logger.LogInformation("🔍 === Consultas Complexas ===");

        // Buscar todos os itens de um cliente (diferentes tipos)
        var todosItens = await _dynamoService.ObterTodosItensClienteAsync(789);
        _logger.LogInformation("📊 Cliente 789 tem {Count} itens total de diferentes tipos", todosItens.Count);

        foreach (var item in todosItens)
        {
            _logger.LogInformation("📋 {Tipo}: {Item}", item.Tipo, item.GetType().Name);
        }

        // Listar todos os itens de produtos dos pedidos
        var itensDosPedidos = await _dynamoService.ListarItensDosPedidosAsync(789);
        _logger.LogInformation("🛒 Cliente tem {Count} itens em todos os pedidos", itensDosPedidos.Count);

        var valorTotalCompras = itensDosPedidos.Sum(i => i.ValorTotal);
        _logger.LogInformation("💰 Valor total em compras: R${Total:F2}", valorTotalCompras);
    }

    private async Task DemonstrarEstatisticasAsync()
    {
        _logger.LogInformation("📈 === Estatísticas da Tabela ===");

        var estatisticas = await _dynamoService.ObterEstatisticasPorTipoAsync();

        _logger.LogInformation("📊 Estatísticas por tipo de entidade:");
        foreach (var stat in estatisticas.OrderByDescending(s => s.Value))
        {
            _logger.LogInformation("  {Tipo}: {Count} registros", stat.Key, stat.Value);
        }

        var totalRegistros = estatisticas.Values.Sum();
        _logger.LogInformation("📋 Total de registros na tabela: {Total}", totalRegistros);
    }
}
