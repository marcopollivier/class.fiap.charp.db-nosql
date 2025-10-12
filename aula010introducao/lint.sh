#!/bin/bash

echo "🔍 Executando análise de código e formatação..."

cd PedidosApi

echo "📝 Formatando código..."
dotnet format --verbosity minimal

echo "🧹 Verificando imports não utilizados..."
dotnet format analyzers --verbosity minimal

echo "✅ Lint concluído!"
