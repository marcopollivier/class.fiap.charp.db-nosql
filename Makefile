.PHONY: format

# Define as pastas de aula que contêm projetos .NET
AULA_DIRS := aula010introducao/PedidosApi

format:
	@for dir in $(AULA_DIRS); do \
		echo "Formatando $$dir..."; \
		cd $$dir && \
		dotnet format --verbosity minimal && \
		dotnet format analyzers --verbosity minimal && \
		dotnet format whitespace --verbosity minimal && \
		dotnet format analyzers --verbosity minimal --diagnostics IDE0005 && \
		cd ../..; \
	done

