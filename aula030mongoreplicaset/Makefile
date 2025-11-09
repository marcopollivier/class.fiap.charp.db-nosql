# Makefile para MongoDB Replica Set

.PHONY: up down clean restart status data

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
	@docker exec mongo-primary mongosh exemploSimples --eval 'db.clientes.find().pretty()' 2>/dev/null || echo "❌ Sem dados"
