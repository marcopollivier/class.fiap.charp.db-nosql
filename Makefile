.PHONY: format

# Define as pastas de aula que contêm projetos .NET
AULA_DIRS := aula010introducao/PedidosApi

deps:
	dotnet tool install -g dotnet-format

prepare:
	@for dir in $(AULA_DIRS); do \
		cd $$dir && \
		echo "Building $$dir..."; \
		dotnet restore && \
		dotnet build && \
		echo "Formatando $$dir..."; \
		dotnet format --verbosity minimal && \
		dotnet format analyzers --verbosity minimal && \
		dotnet format whitespace --verbosity minimal && \
		dotnet format analyzers --verbosity minimal --diagnostics IDE0005 && \
		cd ../..; \
	done
