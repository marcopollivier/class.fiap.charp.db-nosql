// Script de dados de exemplo para demonstrar Change Streams
// Execute: mongosh --file sample-data.js

print("📦 Inserindo dados de exemplo para Data Lake...");

// Usar a database de demonstração
use("datalake_demo");

// Limpar dados existentes
db.clientes.deleteMany({});
db.itens.deleteMany({});
db.pedidos.deleteMany({});
db.datalake.deleteMany({});

print("🧹 Dados anteriores limpos");

// Inserir clientes
const clientes = [
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789abc"),
    nome: "João Silva",
    email: "joao.silva@email.com",
  },
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789abd"),
    nome: "Maria Santos",
    email: "maria.santos@email.com",
  },
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789abe"),
    nome: "Pedro Costa",
    email: "pedro.costa@email.com",
  },
];

db.clientes.insertMany(clientes);
print(`✅ ${clientes.length} clientes inseridos`);

// Inserir itens
const itens = [
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789ac0"),
    nome: "Notebook Dell",
    preco: 2500.0,
    quantidade: 10,
  },
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789ac1"),
    nome: "Mouse Logitech",
    preco: 75.0,
    quantidade: 50,
  },
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789ac2"),
    nome: "Teclado Mecânico",
    preco: 350.0,
    quantidade: 25,
  },
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789ac3"),
    nome: "Monitor 24pol",
    preco: 800.0,
    quantidade: 15,
  },
];

db.itens.insertMany(itens);
print(`✅ ${itens.length} itens inseridos`);

// Inserir pedidos
const pedidos = [
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789ac4"),
    clienteId: ObjectId("6564f1a2b8c9d4e5f6789abc"),
    dataPedido: new Date("2024-11-01T10:00:00Z"),
    itens: [
      {
        itemId: ObjectId("6564f1a2b8c9d4e5f6789ac0"),
        nome: "Notebook Dell",
        preco: 2500.0,
        quantidade: 1,
      },
      {
        itemId: ObjectId("6564f1a2b8c9d4e5f6789ac1"),
        nome: "Mouse Logitech",
        preco: 75.0,
        quantidade: 1,
      },
    ],
    total: 2575.0,
  },
  {
    _id: ObjectId("6564f1a2b8c9d4e5f6789ac5"),
    clienteId: ObjectId("6564f1a2b8c9d4e5f6789abd"),
    dataPedido: new Date("2024-11-02T14:30:00Z"),
    itens: [
      {
        itemId: ObjectId("6564f1a2b8c9d4e5f6789ac2"),
        nome: "Teclado Mecânico",
        preco: 350.0,
        quantidade: 1,
      },
    ],
    total: 350.0,
  },
];

db.pedidos.insertMany(pedidos);
print(`✅ ${pedidos.length} pedidos inseridos`);

// Criar índices para otimização
db.clientes.createIndex({ email: 1 }, { unique: true });
db.pedidos.createIndex({ clienteId: 1 });
db.pedidos.createIndex({ dataPedido: 1 });
db.datalake.createIndex({ timestamp: 1 });
db.datalake.createIndex({ evento: 1 });

print("📊 Índices criados");

// Mostrar resumo dos dados
print("\n📈 RESUMO DOS DADOS:");
print(`- Clientes: ${db.clientes.countDocuments()}`);
print(`- Itens: ${db.itens.countDocuments()}`);
print(`- Pedidos: ${db.pedidos.countDocuments()}`);

print(
  "\n🚀 Dados prontos! Agora execute a aplicação .NET para ver os Change Streams em ação!"
);
print("💡 Execute 'make run' para iniciar o processamento do Data Lake");

// Aguardar um pouco e inserir mais dados para demonstrar Change Streams
print("\n⏳ Aguardando 3 segundos para inserir mais dados...");
sleep(3000);

// Inserir novo pedido para demonstrar Change Stream em ação
const novoPedido = {
  _id: ObjectId("6564f1a2b8c9d4e5f6789ac6"),
  clienteId: ObjectId("6564f1a2b8c9d4e5f6789abe"),
  dataPedido: new Date(),
  itens: [
    {
      itemId: ObjectId("6564f1a2b8c9d4e5f6789ac3"),
      nome: "Monitor 24pol",
      preco: 800.0,
      quantidade: 2,
    },
  ],
  total: 1600.0,
};

db.pedidos.insertOne(novoPedido);
print("🆕 Novo pedido inserido para demonstrar Change Stream!");
