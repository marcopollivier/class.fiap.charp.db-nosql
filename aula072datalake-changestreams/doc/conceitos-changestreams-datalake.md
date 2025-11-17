# Change Streams + Data Lake - Conceitos e Implementação

## Conceitos Fundamentais

### 1. Change Streams

**Change Streams** são uma funcionalidade do MongoDB que permite escutar mudanças em tempo real em:
- Collections específicas
- Databases
- Todo o cluster

#### Características:

- **Tempo Real**: Eventos são processados imediatamente
- **Atomicidade**: Cada evento representa uma operação completa
- **Ordenação**: Eventos são entregues em ordem cronológica
- **Fault Tolerance**: Replica Set garante durabilidade dos eventos

#### Tipos de Operações:

```javascript
{
  "operationType": "insert",    // Novo documento
  "operationType": "update",    // Documento modificado
  "operationType": "delete",    // Documento removido
  "operationType": "replace",   // Documento substituído
  "operationType": "drop",      // Collection removida
  "operationType": "rename",    // Collection renomeada
  "operationType": "invalidate" // Stream invalidado
}
```

### 2. Data Lake Pattern

Um **Data Lake** é um repositório centralizado que armazena dados estruturados e não estruturados em sua forma nativa.

#### Características do nosso Data Lake educacional:

- **Single Source of Truth**: Todos os eventos de mudança ficam centralizados
- **Denormalização**: Dados são pré-processados para consultas analíticas
- **Auditoria**: Histórico completo de mudanças
- **Escalabilidade**: Pode ser estendido para diferentes destinos

## Arquitetura da Solução

### Fluxo de Dados

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Cliente   │────▶│   Pedido    │────▶│ Change      │────▶│ Data Lake   │
│ (Insert)    │     │ (Insert)    │     │ Stream      │     │ (Evento     │
└─────────────┘     └─────────────┘     │ Processor   │     │ Denorm.)    │
                                        └─────────────┘     └─────────────┘
```

### Componentes

1. **Fonte de Dados**: Collections operacionais (clientes, pedidos, itens)
2. **Change Stream Processor**: Serviço .NET que escuta mudanças
3. **Data Lake**: Collection denormalizada para análises
4. **MongoDB Replica Set**: Infraestrutura necessária para Change Streams

## Implementação Técnica

### 1. Configuração do Replica Set

Change Streams requerem MongoDB em modo **Replica Set**:

```yaml
# docker-compose.yml
services:
  mongodb-primary:
    command: mongod --replSet rs0 --bind_ip_all
  mongodb-secondary:
    command: mongod --replSet rs0 --bind_ip_all
```

```javascript
// Inicialização
rs.initiate({
  _id: "rs0",
  members: [
    { _id: 0, host: "mongodb-primary:27017" },
    { _id: 1, host: "mongodb-secondary:27017" }
  ]
})
```

### 2. Monitoramento com C#

```csharp
// Pipeline para filtrar operações
var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Pedido>>()
    .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                   change.OperationType == ChangeStreamOperationType.Update ||
                   change.OperationType == ChangeStreamOperationType.Delete);

// Opções para incluir documento completo
var options = new ChangeStreamOptions
{
    FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
};

// Watch for changes
using var cursor = await collection.WatchAsync(pipeline, options, cancellationToken);
```

### 3. Denormalização

Transformamos dados operacionais em formato analítico:

```json
// Documento Operacional (Pedido)
{
  "_id": ObjectId("..."),
  "clienteId": ObjectId("..."),
  "dataPedido": ISODate("..."),
  "itens": [...],
  "total": 150.00
}

// Documento Data Lake (Denormalizado)
{
  "_id": ObjectId("..."),
  "evento": "pedido_insert",
  "timestamp": ISODate("..."),
  "pedido": {
    "id": ObjectId("..."),
    "total": 150.00,
    "cliente": {
      "nome": "João Silva",      // ← Dados do cliente incluídos
      "email": "joao@email.com"
    },
    "itens": [...],
    "quantidadeItens": 3,        // ← Campos calculados
    "valorMedio": 50.00
  }
}
```

## Casos de Uso Educacionais

### 1. E-commerce Analytics

- **Real-time Dashboard**: Vendas em tempo real
- **Customer Journey**: Rastreamento de ações do cliente
- **Inventory Management**: Alertas de estoque baixo

### 2. Auditoria e Compliance

- **Change Tracking**: Histórico completo de mudanças
- **Data Lineage**: Rastreabilidade de dados
- **Compliance Reports**: Relatórios regulatórios

### 3. Machine Learning

- **Feature Engineering**: Dados denormalizados para ML
- **Real-time Scoring**: Modelos em tempo real
- **Data Preparation**: ETL simplificado

## Vantagens e Limitações

### ✅ Vantagens

1. **Tempo Real**: Processamento imediato de mudanças
2. **Atomicidade**: Consistência garantida por transação
3. **Simplicidade**: Não requer ferramentas ETL complexas
4. **Escalabilidade**: Pode processar milhões de eventos
5. **Fault Tolerance**: Replica Set garante disponibilidade

### ⚠️ Limitações

1. **Replica Set Required**: Não funciona em instância única
2. **MongoDB Specific**: Acoplado ao MongoDB
3. **Overhead**: Cada mudança gera evento adicional
4. **Resume Token**: Tokens expiram após 24h por padrão
5. **Memory Usage**: Pode consumir memória em cenários high-throughput

## Comparação com Outras Abordagens

### vs. ETL Batch

| Aspecto | Change Streams | ETL Batch |
|---------|----------------|-----------|
| **Latência** | Tempo real | Minutos/Horas |
| **Complexidade** | Baixa | Alta |
| **Recursos** | Baixo | Alto |
| **Consistência** | Forte | Eventual |

### vs. Message Queues

| Aspecto | Change Streams | Message Queue |
|---------|----------------|---------------|
| **Setup** | Simples | Complexo |
| **Durabilidade** | MongoDB | Queue específica |
| **Ordering** | Garantido | Depende da impl. |
| **Replay** | Resume Token | Depende da impl. |

## Evolução da Solução

### Próximos Passos (Para produção)

1. **Multi-destination**: Enviar para Elasticsearch, PostgreSQL, etc.
2. **Error Handling**: Retry policies e dead letter queues
3. **Monitoring**: Métricas de performance e alertas
4. **Partitioning**: Separar dados por data/cliente
5. **Compression**: Compactar dados históricos
6. **Schema Evolution**: Versionamento de estruturas

### Extensões Educacionais

1. **Dashboard**: Interface web para visualizar eventos
2. **Alerting**: Notificações para eventos específicos
3. **Testing**: Testes automatizados de Change Streams
4. **Performance**: Benchmarks e otimizações
5. **Multi-tenant**: Separação de dados por cliente

## Referencias e Leitura Adicional

- [MongoDB Change Streams Documentation](https://docs.mongodb.com/manual/changeStreams/)
- [Data Lake Patterns](https://martinfowler.com/bliki/DataLake.html)
- [Event-Driven Architecture](https://microservices.io/patterns/data/event-driven-architecture.html)
- [Change Data Capture](https://en.wikipedia.org/wiki/Change_data_capture)