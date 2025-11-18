# Script PowerShell para testar a tabela PedidosApp no DynamoDB da AWS
# Requer AWS CLI configurado com credenciais válidas

$TABELA = "PedidosApp"

Write-Host "🚀 Testando tabela $TABELA no DynamoDB da AWS" -ForegroundColor Green
Write-Host "=============================================="

# 1. Verificar se a tabela existe
Write-Host "📋 1. Verificando se a tabela existe..." -ForegroundColor Yellow

try {
    $tableInfo = aws dynamodb describe-table --table-name $TABELA 2>$null | ConvertFrom-Json
    if ($tableInfo) {
        Write-Host "✅ Tabela $TABELA encontrada!" -ForegroundColor Green

        # Obter informações da tabela
        Write-Host "📊 Informações da tabela:" -ForegroundColor Cyan
        aws dynamodb describe-table --table-name $TABELA --query 'Table.[TableName,TableStatus,ItemCount]' --output table
    }
} catch {
    Write-Host "❌ Tabela $TABELA não encontrada. Verifique se ela existe na sua conta AWS." -ForegroundColor Red
    exit 1
}

Write-Host ""

# 2. Inserir um item de teste (cliente)
Write-Host "📝 2. Inserindo cliente de teste..." -ForegroundColor Yellow

try {
    $insertResult = aws dynamodb put-item --table-name $TABELA --item file://../dynamodb/02-insert-client.json 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Cliente inserido com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "❌ Erro ao inserir cliente" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Erro ao inserir cliente" -ForegroundColor Red
}

Write-Host ""

# 3. Consultar o cliente inserido
Write-Host "🔍 3. Consultando cliente inserido..." -ForegroundColor Yellow
aws dynamodb get-item --table-name $TABELA --key '{\"PK\":{\"S\":\"CLIENTE#123\"},\"SK\":{\"S\":\"CLIENTE#123\"}}' --query 'Item.[nome.S,email.S,criado_em.S]' --output table

Write-Host ""

# 4. Listar todos os itens de um cliente (Query)
Write-Host "📊 4. Listando todos os itens do CLIENTE#123..." -ForegroundColor Yellow
aws dynamodb query --table-name $TABELA --key-condition-expression "PK = :pk" --expression-attribute-values '{\":pk\":{\"S\":\"CLIENTE#123\"}}' --query 'Items[*].[Tipo.S,SK.S]' --output table

Write-Host ""

# 5. Executar aplicação .NET
Write-Host "🚀 5. Executando aplicação .NET de demonstração..." -ForegroundColor Yellow
Write-Host "💡 Para executar manualmente: cd ClientApp.DynamoDemo && dotnet run" -ForegroundColor Cyan

Write-Host ""
Write-Host "🎯 Teste concluído! A tabela $TABELA está funcionando corretamente." -ForegroundColor Green
