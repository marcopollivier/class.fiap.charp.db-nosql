# Makefile para MongoDB Replica Set

.PHONY: up down clean restart status data check-replication test-app

# Subir ambiente
up:
	@echo "📦 Subindo MongoDB Replica Set..."
	docker compose up -d
	@sleep 8
	@./configure-replica-set.sh

# Parar ambiente
down:
	docker compose down

# Limpar tudo
clean:
	docker compose down -v

# Restart completo
restart: clean up

# Ver status do replica set
status:
	@docker exec mongo-primary mongosh --eval 'rs.status()' 2>/dev/null || echo "❌ Containers não rodando"

# Ver dados
data:
	@docker exec mongo-primary mongosh pedidos --eval 'db.clientes.find().pretty()' 2>/dev/null || echo "❌ Sem dados"

# Verificar replicação em todos os nós
check-replication:
	@echo "🔍 Verificando replicação em todos os nós:"
	@echo "\n📊 PRIMARY (27017):"
	@docker exec mongo-primary mongosh pedidos --eval 'db.clientes.find().count()' 2>/dev/null || echo "❌ Erro"
	@echo "\n📊 SECONDARY 1 (27018):"
	@docker exec mongo-secondary1 mongosh pedidos --eval 'rs.secondaryOk(); db.clientes.find().count()' 2>/dev/null || echo "❌ Erro"
	@echo "\n📊 SECONDARY 2 (27019):"
	@docker exec mongo-secondary2 mongosh pedidos --eval 'rs.secondaryOk(); db.clientes.find().count()' 2>/dev/null || echo "❌ Erro"
	@echo "\n⚙️  Status dos membros:"
	@docker exec mongo-primary mongosh --eval 'rs.status()' 2>/dev/null | grep -E "(name|stateStr|health)" || echo "❌ Erro"

# Executar aplicação .NET
test-app:
	@echo "🚀 Executando aplicação .NET..."
	cd PedidosApiSimples && dotnet run
