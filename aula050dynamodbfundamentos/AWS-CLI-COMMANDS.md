# Comandos AWS CLI para Tabela PedidosApp

Este arquivo contém exemplos práticos de comandos AWS CLI para manipular a tabela `PedidosApp` no DynamoDB.

## 📋 Informações da Tabela

```bash
# Descrever a tabela
aws dynamodb describe-table --table-name PedidosApp

# Verificar status e contagem de itens
aws dynamodb describe-table --table-name PedidosApp \
    --query 'Table.[TableName,TableStatus,ItemCount,BillingModeSummary.BillingMode]' \
    --output table
```

## 📝 Inserção de Dados (PutItem)

```bash
# Inserir cliente usando arquivo JSON
aws dynamodb put-item --table-name PedidosApp --item file://dynamodb/02-insert-client.json

# Inserir perfil
aws dynamodb put-item --table-name PedidosApp --item file://dynamodb/03-insert-perfil.json

# Inserir pedido
aws dynamodb put-item --table-name PedidosApp --item file://dynamodb/04-insert-pedido1.json

# Inserir item do pedido
aws dynamodb put-item --table-name PedidosApp --item file://dynamodb/08-insert-item-pedido.json

# Inserir endereços
aws dynamodb put-item --table-name PedidosApp --item file://dynamodb/06-insert-endereco-residencial.json
aws dynamodb put-item --table-name PedidosApp --item file://dynamodb/07-insert-endereco-comercial.json
```

## 🔍 Consultas (GetItem)

```bash
# Buscar cliente específico
aws dynamodb get-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#123"},"SK":{"S":"CLIENTE#123"}}' \
    --query 'Item.[nome.S,email.S,criado_em.S]' \
    --output table

# Buscar perfil do cliente
aws dynamodb get-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#123"},"SK":{"S":"PROFILE"}}' \
    --query 'Item.[telefone.S,data_nascimento.S]' \
    --output table

# Buscar pedido específico
aws dynamodb get-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#123"},"SK":{"S":"PEDIDO#2024-0001"}}' \
    --query 'Item.[pedido_id.S,status.S,valor_total.N]' \
    --output table
```

## 📊 Queries (Consultas Eficientes)

```bash
# Buscar todos os itens de um cliente
aws dynamodb query \
    --table-name PedidosApp \
    --key-condition-expression "PK = :pk" \
    --expression-attribute-values '{":pk":{"S":"CLIENTE#123"}}' \
    --query 'Items[*].[Tipo.S,SK.S]' \
    --output table

# Buscar apenas pedidos de um cliente
aws dynamodb query \
    --table-name PedidosApp \
    --key-condition-expression "PK = :pk AND begins_with(SK, :sk_prefix)" \
    --filter-expression "#tipo = :tipo" \
    --expression-attribute-names '{"#tipo":"Tipo"}' \
    --expression-attribute-values '{":pk":{"S":"CLIENTE#123"},":sk_prefix":{"S":"PEDIDO#"},":tipo":{"S":"PEDIDO"}}' \
    --query 'Items[*].[pedido_id.S,status.S,valor_total.N,data_pedido.S]' \
    --output table

# Buscar endereços de um cliente
aws dynamodb query \
    --table-name PedidosApp \
    --key-condition-expression "PK = :pk AND begins_with(SK, :sk_prefix)" \
    --expression-attribute-values '{":pk":{"S":"CLIENTE#123"},":sk_prefix":{"S":"ENDERECO#"}}' \
    --query 'Items[*].[SK.S,logradouro.S,numero.S,cidade.S]' \
    --output table

# Buscar itens de pedidos
aws dynamodb query \
    --table-name PedidosApp \
    --key-condition-expression "PK = :pk" \
    --filter-expression "#tipo = :tipo" \
    --expression-attribute-names '{"#tipo":"Tipo"}' \
    --expression-attribute-values '{":pk":{"S":"CLIENTE#123"},":tipo":{"S":"ITEM"}}' \
    --query 'Items[*].[pedido_id.S,nome.S,quantidade.N,preco_unitario.N]' \
    --output table
```

## 🔄 Atualizações (UpdateItem)

```bash
# Atualizar email do cliente
aws dynamodb update-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#123"},"SK":{"S":"CLIENTE#123"}}' \
    --update-expression "SET email = :email" \
    --expression-attribute-values '{":email":{"S":"novo.email@example.com"}}'

# Atualizar status do pedido
aws dynamodb update-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#123"},"SK":{"S":"PEDIDO#2024-0001"}}' \
    --update-expression "SET #status = :status" \
    --expression-attribute-names '{"#status":"status"}' \
    --expression-attribute-values '{":status":{"S":"ENTREGUE"}}'

# Adicionar campo novo (exemplo: data de entrega)
aws dynamodb update-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#123"},"SK":{"S":"PEDIDO#2024-0001"}}' \
    --update-expression "SET data_entrega = :data" \
    --expression-attribute-values '{":data":{"S":"2024-11-17T10:30:00Z"}}'
```

## 🗑️ Remoção (DeleteItem)

```bash
# Remover um item específico (CUIDADO!)
aws dynamodb delete-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#456"},"SK":{"S":"CLIENTE#456"}}'

# Remover com condição de segurança
aws dynamodb delete-item \
    --table-name PedidosApp \
    --key '{"PK":{"S":"CLIENTE#456"},"SK":{"S":"PEDIDO#2025-0001"}}' \
    --condition-expression "#status = :status" \
    --expression-attribute-names '{"#status":"status"}' \
    --expression-attribute-values '{":status":{"S":"CANCELADO"}}'
```

## 📈 Scan (Busca Completa - Use com Cuidado)

```bash
# Contar todos os clientes
aws dynamodb scan \
    --table-name PedidosApp \
    --filter-expression "#tipo = :tipo" \
    --expression-attribute-names '{"#tipo":"Tipo"}' \
    --expression-attribute-values '{":tipo":{"S":"CLIENTE"}}' \
    --select COUNT \
    --query 'Count'

# Listar todos os tipos de entidade
aws dynamodb scan \
    --table-name PedidosApp \
    --query 'Items[*].Tipo.S' \
    --output text | sort | uniq -c

# Buscar clientes por cidade (ineficiente, melhor usar GSI)
aws dynamodb scan \
    --table-name PedidosApp \
    --filter-expression "#tipo = :tipo AND cidade = :cidade" \
    --expression-attribute-names '{"#tipo":"Tipo"}' \
    --expression-attribute-values '{":tipo":{"S":"ENDERECO"},":cidade":{"S":"São Paulo"}}' \
    --query 'Items[*].[PK.S,logradouro.S,numero.S]' \
    --output table
```

## 🔢 Contadores e Estatísticas

```bash
# Contar total de itens na tabela
aws dynamodb scan --table-name PedidosApp --select COUNT --query 'Count'

# Estatísticas por tipo de entidade
aws dynamodb scan \
    --table-name PedidosApp \
    --query 'Items[*].Tipo.S' | jq -r '.[]' | sort | uniq -c

# Valor total de todos os pedidos
aws dynamodb scan \
    --table-name PedidosApp \
    --filter-expression "#tipo = :tipo" \
    --expression-attribute-names '{"#tipo":"Tipo"}' \
    --expression-attribute-values '{":tipo":{"S":"PEDIDO"}}' \
    --query 'Items[*].valor_total.N' \
    --output text | awk '{sum += $1} END {print "Total: R$" sum}'
```

## 🛠️ Backup e Manutenção

```bash
# Criar backup sob demanda
aws dynamodb create-backup \
    --table-name PedidosApp \
    --backup-name "PedidosApp-backup-$(date +%Y%m%d-%H%M%S)"

# Listar backups
aws dynamodb list-backups \
    --table-name PedidosApp \
    --query 'BackupSummaries[*].[BackupName,BackupStatus,BackupCreationDateTime]' \
    --output table

# Exportar dados para S3 (requer configuração prévia)
aws dynamodb export-table-to-point-in-time \
    --table-arn $(aws dynamodb describe-table --table-name PedidosApp --query 'Table.TableArn' --output text) \
    --s3-bucket seu-bucket-backup \
    --s3-prefix dynamodb-exports/PedidosApp/
```

## ⚠️ Comandos de Administração (Use com EXTREMO Cuidado)

```bash
# Limpar todos os dados da tabela (PERIGOSO!)
# Não há comando direto, seria necessário script para scan+delete

# Deletar tabela (MUITO PERIGOSO!)
# aws dynamodb delete-table --table-name PedidosApp

# Restaurar de backup
# aws dynamodb restore-table-from-backup \
#     --target-table-name PedidosApp-restored \
#     --backup-arn arn:aws:dynamodb:region:account:table/PedidosApp/backup/01234567890123-12345678
```

## 💡 Dicas Importantes

1. **Performance**: Prefira `query` sobre `scan` sempre que possível
2. **Custos**: Monitore RCUs/WCUs consumidas com `--return-consumed-capacity TOTAL`
3. **Paginação**: Use `--max-items` e `--starting-token` para grandes consultas
4. **Filtros**: Use `filter-expression` para reduzir dados transferidos
5. **Backup**: Configure backups automáticos em produção
6. **Monitoramento**: Use CloudWatch para acompanhar métricas

## 🔗 Recursos Úteis

- [AWS CLI DynamoDB Reference](https://docs.aws.amazon.com/cli/latest/reference/dynamodb/)
- [DynamoDB Best Practices](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/best-practices.html)
- [Query vs Scan Performance](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/bp-query-scan.html)
