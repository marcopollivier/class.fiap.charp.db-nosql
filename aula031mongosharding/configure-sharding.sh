#!/bin/bash

echo "🚀 Configurando MongoDB Sharded Cluster..."

# Aguardar containers estarem prontos
echo "⏳ Aguardando containers ficarem prontos..."
sleep 15

# Configurar Config Server Replica Set
echo "⚙️ Configurando Config Server Replica Set..."
docker exec -it configsvr1 mongosh --eval "
rs.initiate({
  _id: 'configrs',
  configsvr: true,
  members: [
    {_id: 0, host: 'configsvr1:27017'},
    {_id: 1, host: 'configsvr2:27017'},
    {_id: 2, host: 'configsvr3:27017'}
  ]
})
"

echo "⏳ Aguardando Config Server se estabilizar..."
sleep 20

# Configurar Shard Replica Sets
echo "⚙️ Configurando Shard 1 Replica Set..."
docker exec -it shard1srv mongosh --eval "
rs.initiate({
  _id: 'shard1rs',
  members: [{_id: 0, host: 'shard1srv:27017'}]
})
"

echo "⚙️ Configurando Shard 2 Replica Set..."
docker exec -it shard2srv mongosh --eval "
rs.initiate({
  _id: 'shard2rs', 
  members: [{_id: 0, host: 'shard2srv:27017'}]
})
"

echo "⚙️ Configurando Shard 3 Replica Set..."
docker exec -it shard3srv mongosh --eval "
rs.initiate({
  _id: 'shard3rs',
  members: [{_id: 0, host: 'shard3srv:27017'}]
})
"

echo "⏳ Aguardando Shards se estabilizarem..."
sleep 15

# Adicionar shards ao cluster
echo "🔗 Adicionando shards ao cluster..."
docker exec -it mongos mongosh --eval "
sh.addShard('shard1rs/shard1srv:27017');
sh.addShard('shard2rs/shard2srv:27017');
sh.addShard('shard3rs/shard3srv:27017');
"

# Habilitar sharding no banco
echo "📊 Habilitando sharding no banco 'pedidos'..."
docker exec -it mongos mongosh --eval "sh.enableSharding('pedidos')"

# Configurar shard key
echo "🔑 Configurando shard key na coleção 'clientes'..."
docker exec -it mongos mongosh --eval "
use pedidos;
db.clientes.createIndex({'primeiraLetra': 1});
sh.shardCollection('pedidos.clientes', {'primeiraLetra': 1});
"

# Configurar zonas para distribuição específica (opcional)
echo "🗺️ Configurando zonas para distribuição por inicial..."
docker exec -it mongos mongosh --eval "
sh.addShardTag('shard1rs', 'A-H');
sh.addShardTag('shard2rs', 'I-P');
sh.addShardTag('shard3rs', 'Q-Z');

sh.addTagRange('pedidos.clientes', {'primeiraLetra': 'A'}, {'primeiraLetra': 'I'}, 'A-H');
sh.addTagRange('pedidos.clientes', {'primeiraLetra': 'I'}, {'primeiraLetra': 'Q'}, 'I-P');
sh.addTagRange('pedidos.clientes', {'primeiraLetra': 'Q'}, {'primeiraLetra': 'ZZ'}, 'Q-Z');
"

echo "✅ Configuração do sharding concluída!"
echo "📊 Verificando status do cluster..."

docker exec -it mongos mongosh --eval "sh.status()"

echo ""
echo "🎉 MongoDB Sharded Cluster configurado com sucesso!"
echo "💡 Use 'make test-data' para inserir dados de teste"
echo "📊 Use 'make check-distribution' para verificar distribuição"