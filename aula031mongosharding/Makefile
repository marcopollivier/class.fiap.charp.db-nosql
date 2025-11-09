.PHONY: up down clean logs status setup-sharding mongo-shell

# Configuração principal
up:
	@echo "🚀 Iniciando MongoDB Sharded Cluster..."
	docker compose up -d
	@echo "⏳ Aguardando inicialização dos containers..."
	sleep 10
	@echo "✅ Cluster iniciado! Execute 'make setup-sharding' para configurar o sharding"

# Parar containers
down:
	@echo "🛑 Parando MongoDB Sharded Cluster..."
	docker compose down

# Limpeza completa
clean:
	@echo "🧹 Limpando volumes e containers..."
	docker compose down -v
	docker system prune -f

# Ver logs
logs:
	docker compose logs -f

# Status dos containers
status:
	docker compose ps

# Configurar sharding (executar após 'make up')
setup-sharding:
	@echo "⚙️ Configurando Config Server Replica Set..."
	-docker exec -it configsvr1 mongosh --eval "rs.initiate({_id: 'configrs', configsvr: true, members: [{_id: 0, host: 'configsvr1:27017'}, {_id: 1, host: 'configsvr2:27017'}, {_id: 2, host: 'configsvr3:27017'}]})"
	@echo "⏳ Aguardando Config Server..."
	sleep 15
	
	@echo "⚙️ Configurando Shard 1 Replica Set..."
	-docker exec -it shard1srv mongosh --eval "rs.initiate({_id: 'shard1rs', members: [{_id: 0, host: 'shard1srv:27017'}]})"
	sleep 5
	
	@echo "⚙️ Configurando Shard 2 Replica Set..."
	-docker exec -it shard2srv mongosh --eval "rs.initiate({_id: 'shard2rs', members: [{_id: 0, host: 'shard2srv:27017'}]})"
	sleep 5
	
	@echo "⚙️ Configurando Shard 3 Replica Set..."
	-docker exec -it shard3srv mongosh --eval "rs.initiate({_id: 'shard3rs', members: [{_id: 0, host: 'shard3srv:27017'}]})"
	sleep 10
	
	@echo "🔗 Adicionando shards ao cluster..."
	-docker exec -it mongos mongosh --eval "sh.addShard('shard1rs/shard1srv:27017')"
	-docker exec -it mongos mongosh --eval "sh.addShard('shard2rs/shard2srv:27017')"
	-docker exec -it mongos mongosh --eval "sh.addShard('shard3rs/shard3srv:27017')"
	
	@echo "📊 Habilitando sharding no banco 'pedidos'..."
	-docker exec -it mongos mongosh --eval "sh.enableSharding('pedidos')"
	
	@echo "🔑 Configurando shard key na coleção 'clientes'..."
	-docker exec -it mongos mongosh --eval "use pedidos; db.clientes.createIndex({'primeiraLetra': 1})"
	-docker exec -it mongos mongosh --eval "sh.shardCollection('pedidos.clientes', {'primeiraLetra': 1})"
	
	@echo "✅ Sharding configurado! Verificando status..."
	docker exec -it mongos mongosh --eval "sh.status()"

# Conectar ao mongos via shell
mongo-shell:
	@echo "🐚 Conectando ao Mongos Router..."
	docker exec -it mongos mongosh

# Verificar distribuição de dados
check-distribution:
	@echo "📊 Verificando distribuição de dados..."
	docker exec -it mongos mongosh --eval "use pedidos; db.clientes.getShardDistribution()"

# Inserir dados de teste
test-data:
	@echo "📝 Inserindo dados de teste..."
	docker exec -it mongos mongosh --eval "use pedidos; db.clientes.insertMany([{nome: 'Ana Silva', email: 'ana@teste.com', primeiraLetra: 'A'}, {nome: 'Bruno Costa', email: 'bruno@teste.com', primeiraLetra: 'B'}, {nome: 'Igor Santos', email: 'igor@teste.com', primeiraLetra: 'I'}, {nome: 'Julia Lima', email: 'julia@teste.com', primeiraLetra: 'J'}, {nome: 'Roberto Alves', email: 'roberto@teste.com', primeiraLetra: 'R'}, {nome: 'Sofia Oliveira', email: 'sofia@teste.com', primeiraLetra: 'S'}])"
	@echo "✅ Dados inseridos! Execute 'make check-distribution' para ver a distribuição"

# Help
help:
	@echo "📚 Comandos disponíveis:"
	@echo "  make up              - Iniciar cluster"
	@echo "  make setup-sharding  - Configurar sharding (após up)"
	@echo "  make test-data       - Inserir dados de teste"
	@echo "  make check-distribution - Ver distribuição dos dados"
	@echo "  make mongo-shell     - Conectar ao mongos"
	@echo "  make logs           - Ver logs dos containers"
	@echo "  make status         - Status dos containers"
	@echo "  make down           - Parar cluster"
	@echo "  make clean          - Limpeza completa"