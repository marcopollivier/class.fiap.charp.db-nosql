using MongoDB.Driver;
using PedidosApiSimples;

Console.WriteLine("=== EXEMPLO: MongoDB Sharding - Distribuição por Inicial do Nome ===");
Console.WriteLine();

try
{
    // Conexão ao mongos router
    var client = new MongoClient("mongodb://localhost:27000");
    var database = client.GetDatabase("pedidos");
    var collection = database.GetCollection<Cliente>("clientes");

    Console.WriteLine("🔗 Conectado ao Mongos Router (localhost:27000)");
    Console.WriteLine();

    // Clientes para diferentes shards baseados na inicial do nome
    var clientes = new List<Cliente>
    {
        // Shard 1: A-H
        new Cliente { Nome = "Ana Silva", Email = "ana@teste.com" },
        new Cliente { Nome = "Bruno Costa", Email = "bruno@teste.com" },
        new Cliente { Nome = "Carlos Oliveira", Email = "carlos@teste.com" },
        new Cliente { Nome = "Helena Santos", Email = "helena@teste.com" },
        
        // Shard 2: I-P
        new Cliente { Nome = "Igor Lima", Email = "igor@teste.com" },
        new Cliente { Nome = "Julia Ferreira", Email = "julia@teste.com" },
        new Cliente { Nome = "Maria Souza", Email = "maria@teste.com" },
        new Cliente { Nome = "Pedro Alves", Email = "pedro@teste.com" },
        
        // Shard 3: Q-Z
        new Cliente { Nome = "Roberto Machado", Email = "roberto@teste.com" },
        new Cliente { Nome = "Sofia Rodrigues", Email = "sofia@teste.com" },
        new Cliente { Nome = "Thiago Mendes", Email = "thiago@teste.com" },
        new Cliente { Nome = "Zeca Pagodinho", Email = "zeca@teste.com" }
    };

    Console.WriteLine("📊 DEMONSTRAÇÃO: Distribuição Automática entre Shards");
    Console.WriteLine("Shard 1 (A-H) | Shard 2 (I-P) | Shard 3 (Q-Z)");
    Console.WriteLine("──────────────────────────────────────────────────");

    // Limpar coleção anterior
    await collection.DeleteManyAsync(FilterDefinition<Cliente>.Empty);

    // Inserir clientes e mostrar distribuição
    foreach (var cliente in clientes)
    {
        // Calcular a primeira letra para o sharding
        cliente.CalcularPrimeiraLetra();
        
        Console.WriteLine($"📝 Inserindo: {cliente.Nome} → Shard para letra '{cliente.PrimeiraLetra}'");
        
        await collection.InsertOneAsync(cliente);
    }

    Console.WriteLine();
    Console.WriteLine("✅ Todos os clientes inseridos!");
    Console.WriteLine();

    Console.WriteLine("🔍 CONSULTAS DISTRIBUÍDAS:");
    Console.WriteLine();

    // Consulta específica (vai para shard específico)
    Console.WriteLine("1️⃣ Busca específica por inicial 'A' (vai direto para Shard 1):");
    var clientesA = await collection
        .Find(x => x.PrimeiraLetra == "A")
        .ToListAsync();
    
    foreach (var cliente in clientesA)
    {
        Console.WriteLine($"   → {cliente}");
    }
    Console.WriteLine();

    // Consulta que abrange múltiplos shards
    Console.WriteLine("2️⃣ Busca por email contendo 'silva' (consulta todos os shards):");
    var clientesSilva = await collection
        .Find(x => x.Email.Contains("silva"))
        .ToListAsync();
    
    foreach (var cliente in clientesSilva)
    {
        Console.WriteLine($"   → {cliente}");
    }
    Console.WriteLine();

    // Consulta geral (scatter-gather em todos os shards)
    Console.WriteLine("3️⃣ Listar todos os clientes (scatter-gather em todos os shards):");
    var todosClientes = await collection
        .Find(FilterDefinition<Cliente>.Empty)
        .ToListAsync();

    var gruposPorShard = todosClientes
        .GroupBy(c => GetShardRange(c.PrimeiraLetra))
        .OrderBy(g => g.Key);

    foreach (var grupo in gruposPorShard)
    {
        Console.WriteLine($"   {grupo.Key}: {grupo.Count()} clientes");
        foreach (var cliente in grupo)
        {
            Console.WriteLine($"      → {cliente.Nome} ({cliente.PrimeiraLetra})");
        }
    }

    Console.WriteLine();
    Console.WriteLine("🎯 CONCLUSÃO:");
    Console.WriteLine("✅ MongoDB automaticamente distribuiu os dados entre os 3 shards");
    Console.WriteLine("✅ Consultas específicas vão direto para o shard correto");
    Console.WriteLine("✅ Consultas gerais fazem scatter-gather em todos os shards");
    Console.WriteLine("✅ Escalabilidade horizontal: adicione mais shards conforme necessário");
    
    Console.WriteLine();
    Console.WriteLine("💡 PRÓXIMOS PASSOS:");
    Console.WriteLine("• Execute 'make check-distribution' para ver estatísticas detalhadas");
    Console.WriteLine("• Use 'make mongo-shell' e execute 'sh.status()' para ver o cluster");
    Console.WriteLine("• Teste performance com grandes volumes de dados");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("🔧 TROUBLESHOOTING:");
    Console.WriteLine("• Verifique se o cluster está rodando: make status");
    Console.WriteLine("• Configure o sharding: make setup-sharding");
    Console.WriteLine("• Aguarde a inicialização completa (pode levar 1-2 minutos)");
}

static string GetShardRange(string primeiraLetra)
{
    if (string.Compare(primeiraLetra, "A", StringComparison.Ordinal) >= 0 && 
        string.Compare(primeiraLetra, "H", StringComparison.Ordinal) <= 0)
        return "Shard 1 (A-H)";
    
    if (string.Compare(primeiraLetra, "I", StringComparison.Ordinal) >= 0 && 
        string.Compare(primeiraLetra, "P", StringComparison.Ordinal) <= 0)
        return "Shard 2 (I-P)";
    
    if (string.Compare(primeiraLetra, "Q", StringComparison.Ordinal) >= 0 && 
        string.Compare(primeiraLetra, "Z", StringComparison.Ordinal) <= 0)
        return "Shard 3 (Q-Z)";
    
    return "Shard 1 (A-H)"; // Default
}