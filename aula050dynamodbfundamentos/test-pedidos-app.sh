#!/bin/bash

# Script para testar a tabela PedidosApp no DynamoDB da AWS
# Requer AWS CLI configurado com credenciais válidas

TABELA="PedidosApp"

echo "🚀 Testando tabela $TABELA no DynamoDB da AWS"
echo "=============================================="

# 1. Verificar se a tabela existe
echo "📋 1. Verificando se a tabela existe..."
aws dynamodb describe-table --table-name $TABELA > /dev/null 2>&1

if [ $? -eq 0 ]; then
    echo "✅ Tabela $TABELA encontrada!"

    # Obter informações da tabela
    aws dynamodb describe-table --table-name $TABELA --query 'Table.[TableName,TableStatus,ItemCount,KeySchema[*].[AttributeName,KeyType]]' --output table
else
    echo "❌ Tabela $TABELA não encontrada. Verifique se ela existe na sua conta AWS."
    exit 1
fi

echo ""

# 2. Inserir um item de teste (cliente)
echo "📝 2. Inserindo cliente de teste..."
aws dynamodb put-item \
    --table-name $TABELA \
    --item file://../dynamodb/02-insert-client.json

if [ $? -eq 0 ]; then
    echo "✅ Cliente inserido com sucesso!"
else
    echo "❌ Erro ao inserir cliente"
fi

echo ""

# 3. Consultar o cliente inserido
echo "🔍 3. Consultando cliente inserido..."
aws dynamodb get-item \
    --table-name $TABELA \
    --key '{"PK":{"S":"CLIENTE#123"},"SK":{"S":"CLIENTE#123"}}' \
    --query 'Item.[nome.S,email.S,criado_em.S]' \
    --output table

echo ""

# 4. Listar todos os itens de um cliente (Query)
echo "📊 4. Listando todos os itens do CLIENTE#123..."
aws dynamodb query \
    --table-name $TABELA \
    --key-condition-expression "PK = :pk" \
    --expression-attribute-values '{":pk":{"S":"CLIENTE#123"}}' \
    --query 'Items[*].[Tipo.S,SK.S]' \
    --output table

echo ""

# 5. Scan para contar tipos de entidade
echo "🔢 5. Contando tipos de entidade na tabela..."
aws dynamodb scan \
    --table-name $TABELA \
    --query 'Items[*].Tipo.S' \
    --output text | sort | uniq -c

echo ""
echo "🎯 Teste concluído! A tabela $TABELA está funcionando corretamente."
echo "💡 Para mais testes, execute: dotnet run (no diretório ClientApp.DynamoDemo)"
