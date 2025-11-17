# Makefile para Data Lake com Change Streams

.PHONY: help up down logs clean init-replica sample-data check-datalake run

help: ## Mostra este menu de ajuda
	@echo "Comandos disponíveis:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-15s\033[0m %s\n", $$1, $$2}'

up: ## Sobe o MongoDB Replica Set
	@echo "🚀 Subindo MongoDB Replica Set..."
	docker-compose up -d
	@echo "⏳ Aguardando MongoDB inicializar..."
	@sleep 10
	@make init-replica
	@echo "✅ MongoDB Replica Set configurado!"

down: ## Para todos os serviços
	@echo "🛑 Parando serviços..."
	docker-compose down

logs: ## Mostra logs dos serviços
	docker-compose logs -f

clean: ## Remove todos os containers e volumes
	@echo "🧹 Limpando ambiente..."
	docker-compose down -v --remove-orphans
	docker system prune -f

init-replica: ## Inicializa o Replica Set
	@echo "🔧 Configurando Replica Set..."
	@sleep 5
	docker exec mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin --eval "
		rs.initiate({
			_id: 'rs0',
			members: [
				{ _id: 0, host: 'mongodb-primary:27017' },
				{ _id: 1, host: 'mongodb-secondary:27017' }
			]
		})
	"
	@echo "⏳ Aguardando estabilização do Replica Set..."
	@sleep 10

sample-data: ## Insere dados de exemplo
	@echo "📦 Inserindo dados de exemplo..."
	docker exec mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo --file /docker-entrypoint-initdb.d/sample-data.js
	@echo "✅ Dados de exemplo inseridos!"

check-datalake: ## Verifica dados no Data Lake
	@echo "📊 Verificando Data Lake..."
	docker exec mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo --eval "
		print('=== DADOS OPERACIONAIS ===');
		print('Clientes:', db.clientes.countDocuments());
		print('Pedidos:', db.pedidos.countDocuments());
		print('Itens:', db.itens.countDocuments());
		print('');
		print('=== DATA LAKE ===');
		print('Eventos processados:', db.datalake.countDocuments());
		print('');
		print('=== ÚLTIMOS EVENTOS ===');
		db.datalake.find().sort({timestamp: -1}).limit(3).forEach(doc => {
			print(JSON.stringify(doc, null, 2));
		});
	"

run: ## Executa a aplicação .NET
	@echo "🏃 Executando aplicação Data Lake Processor..."
	cd DataLakeProcessor && dotnet run

build: ## Compila a aplicação
	@echo "🔨 Compilando aplicação..."
	cd DataLakeProcessor && dotnet build

restore: ## Restaura dependências
	@echo "📦 Restaurando dependências..."
	cd DataLakeProcessor && dotnet restore

test-connection: ## Testa conexão com MongoDB
	@echo "🔍 Testando conexão..."
	docker exec mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin --eval "
		rs.status()
	"