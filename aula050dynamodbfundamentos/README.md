# Aula 5: DynamoDB Fundamentals com Tabela PedidosApp na AWS

## 📚 EDUCATIONAL-CONTEXT

Este módulo demonstra operações fundamentais do DynamoDB usando a tabela **PedidosApp** existente numa conta AWS real. O projeto utiliza single-table design pattern, uma abordagem avançada de modelagem NoSQL.

## 🏗️ Estrutura da Tabela PedidosApp

### Chaves Primárias
- **PK (Partition Key)**: `CLIENTE#{cliente_id}` - Agrupa todas as entidades de um cliente
- **SK (Sort Key)**: Identifica o tipo e instância específica da entidade

### Entidades Suportadas

| Tipo | Padrão SK | Descrição |
|------|-----------|-----------|
| `CLIENTE` | `CLIENTE#{cliente_id}` | Dados básicos do cliente |
| `PROFILE` | `PROFILE` | Perfil detalhado (telefone, preferências) |
| `PEDIDO` | `PEDIDO#{pedido_id}` | Informações do pedido |
| `ITEM` | `PEDIDO#{pedido_id}#ITEM#{item_id}` | Itens dos pedidos |
| `ENDERECO` | `ENDERECO#{tipo}` | Endereços (HOME, WORK, etc.) |

## 🚀 Como Executar

### Pré-requisitos
- **.NET 9** instalado
- **Credenciais AWS** configuradas (AWS CLI, perfis, ou variáveis de ambiente)
- **Permissões** para acessar a tabela `PedidosApp` no DynamoDB

### Configuração das Credenciais AWS

#### Opção 1: AWS CLI
```bash
aws configure
```

#### Opção 2: Variáveis de Ambiente
```bash
export AWS_ACCESS_KEY_ID=sua_access_key
export AWS_SECRET_ACCESS_KEY=sua_secret_key
export AWS_DEFAULT_REGION=us-east-1
```

#### Opção 3: Perfil AWS
```bash
export AWS_PROFILE=nome_do_perfil
```

### Execução
```bash
cd ClientApp.DynamoDemo
dotnet run
```

## 🔍 CONCEPT-EXPLANATION: Single-Table Design

### Vantagens
- **Performance**: Todas as operações relacionadas a um cliente em uma única consulta
- **Consistência**: Transações ACID dentro da mesma partition key
- **Custo**: Menos RCUs/WCUs para operações relacionadas

### Trade-offs
- **Complexidade**: Requer planejamento cuidadoso dos access patterns
- **Flexibilidade**: Mudanças no modelo podem ser desafiadoras
- **Consultas**: Alguns tipos de consulta cross-entity são limitados

## 📊 COMPARISON: SQL vs NoSQL

### Modelo Relacional (SQL)
```sql
-- Múltiplas tabelas normalizadas
SELECT c.nome, p.valor_total, i.nome as item_nome
FROM clientes c
JOIN pedidos p ON c.id = p.cliente_id  
JOIN itens i ON p.id = i.pedido_id
WHERE c.id = 123;
```

### Modelo NoSQL (DynamoDB)
```csharp
// Uma única query para todas as entidades do cliente
var request = new QueryRequest
{
    KeyConditionExpression = "PK = :pk",
    ExpressionAttributeValues = { [":pk"] = new("CLIENTE#123") }
};
```

## 🛠️ Scripts de Exemplo

Os scripts na pasta `dynamodb/` demonstram:

1. **01-create-table.json**: Estrutura da tabela
2. **02-insert-client.json**: Cliente básico
3. **03-insert-perfil.json**: Perfil detalhado
4. **04-05-insert-pedido*.json**: Pedidos com diferentes status
5. **06-07-insert-endereco*.json**: Endereços residencial e comercial
6. **08-insert-item-pedido.json**: Item de um pedido

## 🎯 PRACTICAL-APPLICATION

### Casos de Uso Demonstrados

1. **Inserção de dados**: Cliente, perfil, pedidos, itens e endereços
2. **Consultas eficientes**: Por partition key (cliente)
3. **Filtros por tipo**: Usando o atributo `Tipo`
4. **Atualizações**: Modificação de status de pedidos
5. **Consultas complexas**: Relacionamento entre entidades

### Access Patterns Suportados

- ✅ Buscar todos os dados de um cliente
- ✅ Listar pedidos de um cliente
- ✅ Buscar perfil específico
- ✅ Listar endereços por tipo
- ✅ Buscar itens de pedidos específicos
- ⚠️ Buscar pedidos por valor (requer GSI)
- ⚠️ Buscar clientes por cidade (requer GSI)

## 🔒 SECURITY-REVIEW

- **Credenciais**: Nunca committar chaves no código
- **Permissões**: Usar princípio do menor privilégio
- **Região**: Definir região explicitamente em produção
- **Criptografia**: Dados em trânsito e em repouso

## ⚠️ WARNING: Custos AWS

- **Pay-per-request**: Cobrança por operação realizada
- **Consultas**: Queries são mais econômicas que Scans
- **Dados**: Volume de dados transferidos afeta o custo
- **Monitoramento**: Use CloudWatch para acompanhar custos

## 📖 Próximos Passos

1. **Aula 6**: DynamoDB Avançado (GSI, LSI, Streams)
2. **Índices Globais**: Para access patterns não cobertos
3. **DynamoDB Streams**: Para auditoria e sincronização
4. **Backup e Recovery**: Estratégias de proteção de dados

---

## 🏷️ Tags Educacionais

- `#single-table-design` - Padrão de modelagem avançado
- `#aws-dynamodb` - Serviço gerenciado da AWS  
- `#nosql-modeling` - Modelagem não-relacional
- `#partition-key` - Distribuição de dados
- `#sort-key` - Ordenação e consultas
