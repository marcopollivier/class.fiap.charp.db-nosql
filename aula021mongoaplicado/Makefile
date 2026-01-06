# Makefile para Aula021 - MongoDB Aplicado

.PHONY: help up down restart logs clean build api-logs mongo-shell

# Variáveis
COMPOSE_FILE = docker-compose.yml
API_CONTAINER = aula021-pedidos-api
MONGO_CONTAINER = aula021-mongodb

help: ## Mostra esta ajuda
	@echo "Comandos disponíveis:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-15s %s\n", $$1, $$2}'

up: ## Sobe todos os serviços (MongoDB + API)
	docker-compose up -d
	@echo "Serviços iniciados!"
	@echo "API: http://localhost:5021"
	@echo "Swagger: http://localhost:5021/swagger"

down: ## Para todos os serviços
	docker-compose down

restart: ## Reinicia todos os serviços
	docker-compose restart

build: ## Reconstrói as imagens
	docker-compose build --no-cache

logs: ## Mostra logs de todos os serviços
	docker-compose logs -f

api-logs: ## Mostra apenas logs da API
	docker-compose logs -f $(API_CONTAINER)

mongo-logs: ## Mostra apenas logs do MongoDB
	docker-compose logs -f $(MONGO_CONTAINER)

mongo-shell: ## Acessa o shell do MongoDB
	docker exec -it $(MONGO_CONTAINER) mongosh --username admin --password password123 --authenticationDatabase admin

clean: ## Para e remove todos os containers e volumes
	docker-compose down -v
	docker system prune -f

status: ## Mostra status dos serviços
	docker-compose ps

# Comandos de desenvolvimento
dev-up: ## Sobe apenas MongoDB (para desenvolvimento local da API)
	docker-compose up -d mongodb

dev-api: ## Roda API localmente (requer .NET 9)
	cd PedidosApi && dotnet run

dev-watch: ## Roda API em modo watch (hot reload)
	cd PedidosApi && dotnet watch run

# Comandos úteis para MongoDB
mongo-create-indexes: ## Cria índices no MongoDB
	docker exec -it $(MONGO_CONTAINER) mongosh --username admin --password password123 --authenticationDatabase admin --eval "use pedidos; db.clientes.createIndex({email: 1}, {unique: true}); db.pedidos.createIndex({clienteId: 1}); db.pedidos.createIndex({dataPedido: -1});"

mongo-stats: ## Mostra estatísticas do MongoDB
	docker exec -it $(MONGO_CONTAINER) mongosh --username admin --password password123 --authenticationDatabase admin --eval "use pedidos; db.stats();"
