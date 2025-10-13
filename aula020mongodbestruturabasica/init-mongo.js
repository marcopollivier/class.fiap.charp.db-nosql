// Script de inicialização do MongoDB
// Este script é executado automaticamente quando o container é criado

// Criar e usar o banco de dados pedidos
db = db.getSiblingDB("pedidos");

// Inserir dados na collection clientes
db.clientes.insertOne({
  _id: "123",
  nome: "João Silva",
  email: "joao@exemplo.com",
});

print("✅ Collection 'clientes' criada com dados iniciais");

// Inserir dados na collection pedidos
db.pedidos.insertOne({
  cliente_id: "123",
  data: new Date(),
  itens: [
    { produto: "Notebook", quantidade: 1, preco: 4500 },
    { produto: "Mouse", quantidade: 1, preco: 150 },
  ],
  total: 4650,
});

print("✅ Collection 'pedidos' criada com dados iniciais");
print("✅ Banco de dados 'pedidos' inicializado com sucesso!");
