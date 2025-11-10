# Makefile para Aula 04 - Redis

.PHONY: help up down clean build run test logs redis-cli docs

help: ## Mostra esta ajuda
	@echo "Comandos disponíveis:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-15s\033[0m %s\n", $$1, $$2}'

up: ## Sobe a infraestrutura Redis
	docker-compose up -d
	@echo "✅ Redis rodando em localhost:6379"
	@echo "📚 Material didático em: ./doc/"
	@echo "🚀 Para rodar a API: make run"

down: ## Para e remove os containers
	docker-compose down

clean: ## Para containers e remove volumes
	docker-compose down -v
	docker system prune -f

build: ## Compila o projeto .NET
	dotnet build RedisExamplesApi.csproj

run: ## Executa a aplicação .NET (certifique-se de que Redis esteja rodando)
	@echo "🚀 Iniciando aplicação..."
	@echo "📖 Swagger UI: http://localhost:5000"
	@echo "❤️  Health Check: http://localhost:5000/health"
	dotnet run --project RedisExamplesApi.csproj

test: ## Executa testes básicos no Redis
	@echo "🧪 Testando conectividade com Redis..."
	docker exec -it aula04-redis redis-cli -a password123 ping
	@echo "📊 Informações do Redis:"
	docker exec -it aula04-redis redis-cli -a password123 info server

redis-cli: ## Abre o Redis CLI
	@echo "🔧 Conectando ao Redis CLI..."
	@echo "💡 Dica: Use 'exit' para sair"
	docker exec -it aula04-redis redis-cli -a password123

logs: ## Mostra logs dos containers
	docker-compose logs -f

docs: ## Abre a documentação no navegador (se disponível)
	@echo "📚 Material didático disponível em:"
	@echo "  - ./doc/README.md (índice principal)"
	@echo "  - ./doc/01-introducao-redis.md"
	@echo "  - ./doc/02-tipos-dados.md"
	@echo "  - ./doc/03-cache-distribuido.md"
	@echo "  - ./doc/04-persistencia.md"
	@echo "  - ./doc/05-casos-uso-avancados.md"
	@echo "  - ./doc/06-resumo-melhores-praticas.md"

# Comandos de exemplo para demonstração
exemplo-cache: ## Testa cache básico via curl
	@echo "🧪 Testando cache de produto..."
	curl -s "http://localhost:5000/api/redisexamples/cache-produto/123" | jq .

exemplo-contador: ## Testa contador de visitas
	@echo "🧪 Incrementando visitas..."
	curl -X POST "http://localhost:5000/api/redisexamples/incrementar-visitas/123"

exemplo-leaderboard: ## Testa leaderboard
	@echo "🧪 Adicionando pontuação..."
	curl -X POST -H "Content-Type: application/json" \
		-d '{"jogador":"Player1","pontos":1500}' \
		"http://localhost:5000/api/redisexamples/leaderboard"
	@echo "\n🏆 Top 10:"
	curl -s "http://localhost:5000/api/redisexamples/leaderboard/top/10" | jq .

status: ## Mostra status da infraestrutura
	@echo "📊 Status dos containers:"
	docker-compose ps
	@echo "\n❤️  Health check da aplicação:"
	curl -s http://localhost:5000/health | jq . || echo "API não está rodando"

quick-start: up ## Início rápido - sobe tudo e mostra próximos passos
	@sleep 3
	@echo "\n🎉 Ambiente pronto!"
	@echo "\n📚 Para estudar:"
	@echo "  make docs         # Ver material didático"
	@echo "\n🚀 Para executar:"
	@echo "  make run          # Iniciar API .NET"
	@echo "\n🧪 Para testar:"
	@echo "  make redis-cli    # Conectar ao Redis"
	@echo "  make exemplo-*    # Executar exemplos"
	@echo "\n📊 Para monitorar:"
	@echo "  make status       # Ver status geral"
	@echo "  make logs         # Ver logs"
