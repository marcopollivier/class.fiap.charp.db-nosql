// Comandos para testar o Data Lake durante a execução
// Execute no mongo shell enquanto a aplicação está rodando

// Conectar ao MongoDB
// mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo

// ========================================
// COMANDOS PARA GERAR EVENTOS NO DATA LAKE
// ========================================

// 1. Inserir um novo cliente
db.clientes.insertOne({
  nome: "Ana Costa",
  email: "ana.costa@email.com",
});

// 2. Inserir novo pedido (irá gerar evento denormalizado)
db.pedidos.insertOne({
  clienteId: ObjectId("6564f1a2b8c9d4e5f6789abc"), // João Silva
  dataPedido: new Date(),
  itens: [
    {
      itemId: ObjectId("6564f1a2b8c9d4e5f6789ac0"),
      nome: "Notebook Dell",
      preco: 2500.0,
      quantidade: 1,
    },
  ],
  total: 2500.0,
});

// 3. Atualizar um pedido existente
db.pedidos.updateOne(
  { _id: ObjectId("6564f1a2b8c9d4e5f6789ac4") },
  {
    $set: {
      total: 2600.0,
      "itens.$[elem].preco": 2525.0,
    },
  },
  { arrayFilters: [{ "elem.nome": "Notebook Dell" }] }
);

// 4. Inserir um item com preço promocional
db.itens.insertOne({
  nome: "SSD 1TB",
  preco: 450.0,
  quantidade: 20,
});

// 5. Atualizar estoque de um item
db.itens.updateOne(
  { nome: "Mouse Logitech" },
  { $inc: { quantidade: -5 }, $set: { preco: 70.0 } }
);

// 6. Deletar um cliente (teste de delete)
// db.clientes.deleteOne({ email: "ana.costa@email.com" });

// ========================================
// CONSULTAS PARA VERIFICAR O DATA LAKE
// ========================================

// Ver todos os eventos processados
print("=== TODOS OS EVENTOS ===");
db.datalake
  .find()
  .sort({ timestamp: -1 })
  .forEach((doc) => {
    print(
      `Evento: ${doc.evento}, Timestamp: ${doc.timestamp}, Operação: ${doc.operacao}`
    );
  });

// Ver eventos específicos de pedidos
print("\n=== EVENTOS DE PEDIDOS ===");
db.datalake
  .find({ evento: /^pedido_/ })
  .sort({ timestamp: -1 })
  .forEach((doc) => {
    if (doc.pedido) {
      print(
        `${doc.evento}: Cliente ${doc.pedido.cliente.nome}, Total: R$ ${doc.pedido.total}`
      );
    } else {
      print(`${doc.evento}: Documento deletado`);
    }
  });

// Estatísticas do Data Lake
print("\n=== ESTATÍSTICAS ===");
const stats = [
  { tipo: "Total de eventos", count: db.datalake.countDocuments() },
  {
    tipo: "Eventos de pedidos",
    count: db.datalake.countDocuments({ evento: /^pedido_/ }),
  },
  {
    tipo: "Eventos de clientes",
    count: db.datalake.countDocuments({ evento: /^cliente_/ }),
  },
  {
    tipo: "Eventos de itens",
    count: db.datalake.countDocuments({ evento: /^item_/ }),
  },
];

stats.forEach((stat) => {
  print(`${stat.tipo}: ${stat.count}`);
});

// ========================================
// CONSULTAS ANALÍTICAS EXEMPLO
// ========================================

// Análise de vendas por cliente
print("\n=== ANÁLISE DE VENDAS POR CLIENTE ===");
db.datalake
  .aggregate([
    { $match: { evento: "pedido_insert" } },
    {
      $group: {
        _id: "$pedido.cliente.nome",
        totalVendas: { $sum: "$pedido.total" },
        quantidadePedidos: { $sum: 1 },
        ticketMedio: { $avg: "$pedido.total" },
      },
    },
    { $sort: { totalVendas: -1 } },
  ])
  .forEach((result) => {
    print(`Cliente: ${result._id}`);
    print(`  Total: R$ ${result.totalVendas.toFixed(2)}`);
    print(`  Pedidos: ${result.quantidadePedidos}`);
    print(`  Ticket Médio: R$ ${result.ticketMedio.toFixed(2)}`);
  });

// Timeline de eventos
print("\n=== TIMELINE DE EVENTOS (ÚLTIMOS 10) ===");
db.datalake
  .find()
  .sort({ timestamp: -1 })
  .limit(10)
  .forEach((doc) => {
    const time = doc.timestamp.toISOString().substring(11, 19);
    print(`[${time}] ${doc.evento} - ${doc.operacao} em ${doc.colecao}`);
  });

print(
  "\n✅ Consultas executadas! Observe os logs da aplicação .NET para ver o processamento em tempo real."
);
