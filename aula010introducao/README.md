# Aula 1: SQL vs NoSQL na Prática

> **Objetivo**: Entender as diferenças fundamentais entre bancos relacionais e NoSQL através de um exemplo prático: sistema de pedidos implementado nos dois paradigmas simultaneamente.

## 🤔 A Pergunta Central

"Por que não usar SQL Server para tudo?"

Nesta aula você vai descobrir quando cada tecnologia brilha através de um comparativo direto usando uma **API única** que persiste nos dois bancos.

## 💡 O que Você Vai Aprender

### Conceitos Fundamentais

- **As 4 famílias NoSQL**: Documento, Chave-Valor, Coluna, Grafo
- **CAP Theorem**: Por que você não pode ter tudo
- **Trade-offs reais**: Performance vs Consistência vs Flexibilidade

### Comparação Prática

- **API unificada**: Um controller que persiste em ambos os bancos
- **Estrutura de dados**: Tabelas normalizadas vs Documentos aninhados  
- **IDs diferentes**: Auto-increment vs ObjectId
- **Modelagem**: Relacional vs Embedded Documents

## 🏗️ Arquitetura Simplificada

```text
Sistema de Pedidos Unificado
├── PedidosController (único)
│   └── PedidosService
│       ├── SqlRepository → SQL Server
│       └── MongoRepository → MongoDB
│
├── SQL Server (Relacional)
│   ├── Tabela: Clientes (Id int, Nome, Email)
│   ├── Tabela: Pedidos (Id int, ClienteId, DataPedido)
│   └── Tabela: Itens (Id int, PedidoId, Nome, Preco, Quantidade)
│
└── MongoDB (Documento)
    ├── Collection: clientes {_id: ObjectId, nome, email}
    └── Collection: pedidos {_id: ObjectId, clienteId, itens: [...]}
```

### Por que Esta Arquitetura?

**Simplicidade Educacional:**

- Um único endpoint cria dados em ambos os bancos
- Resposta mostra IDs de ambos para comparação
- Demonstra diferenças na prática sem complexidade

**Comparação Direta:**

- Consultas diretas sem JOINs
- Flexibilidade de schema  
- Escalabilidade horizontal nativa

## 🚀 Como Executar

### 1. Subir a Infraestrutura

```bash
# Na pasta da aula
docker compose up -d
```

Isso vai subir:

- **SQL Server**: `localhost:1433`
- **MongoDB**: `localhost:27017`

### 2. Executar a API

```bash
dotnet run --project PedidosApi
```

### 3. Testar com Requisições HTTP

Use o arquivo `PedidosApi.http` ou acesse: <http://localhost:5000/swagger>

## 🧪 Endpoints da API Unificada

### Comparação Educacional

```http
GET /api/pedidos/comparacao  # Explicação das diferenças
```

### Operações Unificadas

```http
POST /api/pedidos/clientes              # Cria em ambos os bancos
GET  /api/pedidos/clientes/{mongo}/{sql} # Busca em ambos
POST /api/pedidos/pedidos               # Cria em ambos  
GET  /api/pedidos/pedidos/{mongo}/{sql}  # Busca em ambos
```

### Exemplo de Resposta (Cliente)

```json
{
  "mongoId": "675d0a1b2c3d4e5f60789012",
  "sqlId": 1,
  "message": "Cliente criado em ambos os bancos de dados"
}
```

## 🔍 Experimentos Sugeridos

### 1. Compare a Estrutura de Dados

```sql
-- SQL: Veja as tabelas relacionadas
SELECT * FROM Clientes
SELECT * FROM Pedidos 
SELECT * FROM Itens
```

```javascript
// MongoDB: Veja o documento completo
db.pedidos.findOne()
```

### 2. Compare as Queries

- **SQL**: JOIN entre 3 tabelas para buscar um pedido completo
- **MongoDB**: Busca direta por ID do documento

### 3. Teste Performance

- Crie vários pedidos e meça o tempo de resposta
- Compare consultas complexas vs agregações

## 📝 Arquivos de Teste

Use os arquivos `.http` para testar:

- `PedidosApiMSSQL.http` - Exemplos para SQL Server
- `PedidosApiMongo.http` - Exemplos para MongoDB

## 🎯 Principais Insights

Após executar os testes, você vai perceber:

1. **Flexibilidade**: MongoDB permite mudanças de schema mais facilmente
2. **Consistência**: SQL Server garante integridade referencial
3. **Performance**: Depende do caso de uso (JOINs vs documentos grandes)
4. **Complexidade**: SQL requer mais código para relacionamentos

## 💭 Questões para Reflexão

- Quando você escolheria MongoDB? E SQL Server?
- Como a estrutura dos dados influencia a performance?
- Qual abordagem é mais fácil de manter?
- Como seria escalar cada solução?
