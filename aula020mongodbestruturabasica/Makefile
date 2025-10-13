CONTAINER_NAME = aula02-mongodb

.PHONY: up down

up: ## Iniciar MongoDB com dados limpos
	@echo "🚀 Iniciando MongoDB com dados pré-carregados..."
	@docker compose up -d
	@echo "⏳ Aguardando MongoDB inicializar..."
	@sleep 5
	@echo "✅ MongoDB iniciado com sucesso!"

down: ## Parar MongoDB e limpar dados
	@echo "🛑 Parando MongoDB..."
	@docker compose down --remove-orphans 2>/dev/null || true
	@docker container rm $(CONTAINER_NAME) 2>/dev/null || true
	@docker volume prune -f >/dev/null 2>&1 || true
	@echo "🗑️  Limpando dados locais..."
	@find ./data/db -mindepth 1 -not -name '.gitkeep' -delete 2>/dev/null || true
	@mkdir -p ./data/db
	@echo "✨ Limpeza concluída!"
