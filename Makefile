.PHONY: lint format

format:
	dotnet format --verbosity minimal
	dotnet format analyzers --verbosity minimal

lint:
	cd aula010introducao/PedidosApi && dotnet format --verbosity minimal
	cd aula010introducao/PedidosApi && dotnet format analyzers --verbosity minimal

