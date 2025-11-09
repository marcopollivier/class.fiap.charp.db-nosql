using MongoDB.Driver;

using PedidosApiSimples;

Console.WriteLine("=== EXEMPLO: Persistência com Replica Set ===");
Console.WriteLine();

try
{
    // Conexão simples primeiro
    var client = new MongoClient("mongodb://localhost:27017");
    var database = client.GetDatabase("pedidos");
    var collection = database.GetCollection<Cliente>("clientes");

    var cliente1 = new Cliente
    {
        Nome = "João Silva",
        Email = "joao@teste.com"
    };

    var cliente2 = new Cliente
    {
        Nome = "Maria Santos",
        Email = "maria@teste.com"
    };

    Console.WriteLine("📝 PERSISTINDO SEM TRANSAÇÃO:");
    await collection.InsertOneAsync(cliente1);
    Console.WriteLine($"✅ Cliente salvo com ID: {cliente1.Id}");
    Console.WriteLine();

    Console.WriteLine("🔄 PERSISTINDO COM TRANSAÇÃO:");
    using var session = await client.StartSessionAsync();

    try
    {
        session.StartTransaction();
        await collection.InsertOneAsync(session, cliente2);
        await session.CommitTransactionAsync();
        Console.WriteLine($"✅ Cliente salvo com transação, ID: {cliente2.Id}");
        Console.WriteLine("🎉 AGORA FUNCIONOU! Replica set permite transações!");
    }
    catch (Exception ex)
    {
        await session.AbortTransactionAsync();
        Console.WriteLine($"❌ Erro na transação: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine("🎯 CONCLUSÃO:");
    Console.WriteLine("• Com replica set: transações FUNCIONAM!");
    Console.WriteLine("• Sem replica set: transações NÃO funcionam!");
    Console.WriteLine("• Para 1 operação: ainda não há diferença prática");
    Console.WriteLine("• Diferença aparece com MÚLTIPLAS operações");

    var count = await collection.CountDocumentsAsync("{}");
    Console.WriteLine($"📊 Total de clientes: {count}");
}
catch (Exception ex)
{
    Console.WriteLine($"💥 Erro de conexão: {ex.Message}");
    Console.WriteLine("🔧 Verifique se o MongoDB está rodando em localhost:27017");
}
