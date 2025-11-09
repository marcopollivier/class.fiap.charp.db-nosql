# Aula 1: SQL vs NoSQL na Prática

> **Objetivo**: Entender as diferenças fundamentais entre bancos relacionais e NoSQL através de um exemplo prático: sistema de pedidos implementado nos dois paradigmas.

## 🤔 A Pergunta Central

"Por que não usar SQL Server para tudo?"

Nesta aula você vai descobrir quando cada tecnologia brilha através de um comparativo direto.

## 💡 O que Você Vai Aprender

### Conceitos Fundamentais

- **As 4 famílias NoSQL**: Documento, Chave-Valor, Coluna, Grafo
- **CAP Theorem**: Por que você não pode ter tudo
- **Trade-offs reais**: Performance vs Consistência vs Flexibilidade

### Comparação Prática

- **Mesmo sistema, bancos diferentes**: Sistema de pedidos em SQL Server e MongoDB
- **Estrutura de dados**: Tabelas normalizadas vs Documentos aninhados
- **Queries**: SQL vs agregações MongoDB
- **Performance**: Quando cada um é mais eficiente

## 🏗️ Arquitetura do Projeto

```text
Sistema de Pedidos
├── SQL Server (Relacional)
│   ├── Tabela: Clientes
│   ├── Tabela: Pedidos  
│   └── Tabela: Itens
└── MongoDB (Documento)
    ├── Collection: clientes
    └── Collection: pedidos (com itens embedded)
```

### Por que Esta Comparação?

**SQL Server (Relacional):**

- Dados normalizados em tabelas separadas
- JOINs para relacionar informações
- ACID transactions garantidas
- Schema rígido e tipado

**MongoDB (Documento):**

- Pedidos com itens aninhados (embedded)
- Consultas diretas sem JOINs
- Flexibilidade de schema
- Escalabilidade horizontal nativa

## 🚀 Como Executar

### 1. Subir a Infraestrutura

```bash
# Na pasta da aula
docker-compose up -d
```

Isso vai subir:

- **SQL Server**: `localhost:1433`
- **MongoDB**: `localhost:27017`

### 2. Executar a API

```bash
dotnet run --project PedidosApi
```

### 3. Testar no Swagger

Acesse: <http://localhost:5209/swagger>

## 🧪 Endpoints para Comparação

### SQL Server (Relacional)

```http
POST /api/sql/clientes     # Criar cliente
GET  /api/sql/clientes/{id} # Buscar cliente
POST /api/sql/pedidos      # Criar pedido
GET  /api/sql/pedidos/{id}  # Buscar pedido (com JOIN)
```

### MongoDB (Documento)

```http
POST /api/mongo/clientes     # Criar cliente
GET  /api/mongo/clientes/{id} # Buscar cliente
POST /api/mongo/pedidos      # Criar pedido
GET  /api/mongo/pedidos/{id}  # Buscar pedido (direto)
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
