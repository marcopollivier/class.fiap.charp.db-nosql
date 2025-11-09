#!/bin/bash

echo "⚙️ Configurando replica set..."
sleep 3

# Configura replica set
docker exec mongo-primary mongosh --eval '
config = {
  "_id": "rs0",
  "members": [
    {"_id": 0, "host": "mongo-primary:27017", "priority": 2},
    {"_id": 1, "host": "mongo-secondary1:27017"},
    {"_id": 2, "host": "mongo-secondary2:27017"}
  ]
};
try {
  rs.initiate(config);
  print("✅ Replica set configurado!");
} catch (e) {
  print("⚠️ Replica set já existe");
}
' > /dev/null

sleep 5

# Insere dados de exemplo
docker exec mongo-primary mongosh exemploSimples --eval '
db.clientes.deleteMany({});
db.clientes.insertOne({nome: "João Silva", email: "joao@teste.com"});
db.clientes.insertOne({nome: "Maria Santos", email: "maria@teste.com"});
print("✅ Dados inseridos!");
' > /dev/null

echo "🎉 Pronto! Use: mongodb://localhost:27017/?directConnection=true"
