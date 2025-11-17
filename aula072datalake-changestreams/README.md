# Aula 072 - Data Lake com MongoDB Change Streams

## Conceito Educacional

Este projeto demonstra como usar **MongoDB Change Streams** para criar um pipeline simples de Data Lake, onde mudanças em tempo real são capturadas e processadas.

## Arquitetura Didática

```
┌─────────────────┐    Change Stream    ┌─────────────────┐    Process     ┌─────────────────┐
│   MongoDB       │ ─────────────────► │  .NET Service   │ ──────────────► │   Data Lake     │
│   (Operacional) │                    │  (Processor)    │                │   (Analytics)   │
└─────────────────┘                    └─────────────────┘                └─────────────────┘
```

### Fluxo de Dados

1. **Fonte Operacional**: MongoDB com collections de Pedidos, Clientes e Itens
2. **Change Streams**: Monitora mudanças em tempo real (insert, update, delete)
3. **Processamento**: Serviço .NET captura e transforma os dados
4. **Destino Analytics**: Collection "datalake" com dados denormalizados para análise

## Tecnologias

- **.NET 9**: Serviço de processamento
- **MongoDB**: Banco operacional + destino analytics
- **Docker**: Ambiente de desenvolvimento
- **Change Streams**: Recurso nativo do MongoDB para CDC (Change Data Capture)

## Estrutura do Projeto

```
aula072datalake-changestreams/
├── docker-compose.yml          # MongoDB Replica Set (necessário para Change Streams)
├── DataLakeProcessor/          # Aplicação .NET
│   ├── Program.cs
│   ├── Models/                 # Modelos padronizados (Cliente, Pedido, Item)
│   ├── Services/               # Serviços de processamento
│   └── DataLakeProcessor.csproj
├── scripts/
│   ├── init-replica-set.sh     # Configuração do Replica Set
│   └── sample-data.js          # Dados de exemplo
└── README.md
```

## Conceitos Demonstrados

### 1. Change Streams
- Monitoramento em tempo real de mudanças
- Pipelines de aggregation para filtrar eventos
- Tratamento de operações CRUD

### 2. Data Lake Pattern
- Captura de dados operacionais
- Transformação para formato analytics
- Denormalização para consultas otimizadas

### 3. Casos de Uso Educacionais
- **E-commerce Analytics**: Análise de pedidos em tempo real
- **Customer Journey**: Rastreamento de ações do cliente
- **Business Intelligence**: Métricas e dashboards

## Quick Start

```bash
# 1. Subir MongoDB Replica Set
make up

# 2. Executar aplicação
make run

# 3. Inserir dados de teste
make sample-data

# 4. Verificar Data Lake
make check-datalake
```

## Exemplos Práticos

### Eventos Capturados

```json
{
  "operationType": "insert",
  "documentKey": { "_id": ObjectId("...") },
  "fullDocument": {
    "_id": ObjectId("..."),
    "clienteId": ObjectId("..."),
    "itens": [...],
    "dataPedido": ISODate("..."),
    "total": 150.00
  }
}
```

### Dados Transformados (Data Lake)

```json
{
  "_id": ObjectId("..."),
  "evento": "pedido_criado",
  "timestamp": ISODate("..."),
  "pedido": {
    "id": ObjectId("..."),
    "total": 150.00,
    "cliente": {
      "nome": "João Silva",
      "email": "joao@email.com"
    },
    "itens": [
      {
        "nome": "Notebook",
        "preco": 150.00,
        "quantidade": 1
      }
    ]
  },
  "metadados": {
    "origem": "change_stream",
    "versao": "1.0"
  }
}
```

## Objetivos de Aprendizado

1. **Change Data Capture**: Como capturar mudanças em tempo real
2. **Data Pipeline**: Criação de pipelines de transformação
3. **Replica Set**: Configuração necessária para Change Streams
4. **Denormalização**: Otimização para consultas analytics
5. **Real-time Processing**: Processamento de eventos em tempo real

## Limitações Educacionais

- **Não é production-ready**: Foco em aprendizado
- **Sem persistência externa**: Data Lake também no MongoDB
- **Sem tratamento de erros complexos**: Código simplificado
- **Sem escalabilidade**: Single instance para demonstração

## Próximos Passos

- Implementar retry policies
- Adicionar diferentes destinos (Elasticsearch, PostgreSQL)
- Criar dashboards com os dados do Data Lake
- Implementar particionamento por data
- Adicionar métricas e monitoramento