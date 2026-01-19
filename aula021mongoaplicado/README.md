# Aula 021 - MongoDB Aplicado

<EDUCATIONAL-CONTEXT>
Esta aula demonstra a aplicação prática do MongoDB em uma API .NET, explorando operações CRUD, modelagem de documentos e características específicas de bancos NoSQL orientados a documentos.
</EDUCATIONAL-CONTEXT>

## 📚 Objetivos de Aprendizagem

- Implementar uma API completa usando MongoDB
- Compreender modelagem de documentos embarcados
- Aplicar operações CRUD em banco de documentos
- Explorar características específicas do MongoDB (ObjectId, BSON, etc.)
- Comparar com abordagens relacionais tradicionais

## 🏗️ Arquitetura da Aplicação

```
PedidosApi/
├── Models/           # Entidades de domínio com atributos MongoDB
├── Repositories/     # Acesso a dados MongoDB
├── Services/        # Lógica de negócio
├── Controllers/     # Endpoints da API
├── Extensions/      # Configuração de serviços
└── Properties/      # Configurações da aplicação
```

## 🔧 Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **MongoDB 7.0** - Banco de dados NoSQL
- **MongoDB.Driver 3.5.0** - Driver oficial para .NET
- **ASP.NET Core Web API** - Framework web
- **Docker** - Containerização
- **Swagger/OpenAPI** - Documentação da API

## 🚀 Como Executar

### Pré-requisitos
- Docker e Docker Compose
- .NET 9 SDK (para desenvolvimento local)

### Usando Docker (Recomendado)
```bash
# Subir todos os serviços
make up

# Ou usando docker compose diretamente
docker compose up -d
```

### Desenvolvimento Local
```bash
# Subir apenas MongoDB
make dev-up

# Executar API localmente
make dev-api

# Ou com hot reload
make dev-watch
```

## 📊 Modelagem de Dados

<CONCEPT-EXPLANATION>
O MongoDB utiliza documentos BSON (Binary JSON) para armazenar dados, permitindo estruturas mais flexíveis que bancos relacionais.
</CONCEPT-EXPLANATION>

### Cliente
```json
{
  "_id": ObjectId("..."),
  "nome": "João da Silva",
  "email": "joao@exemplo.com"
}
```

### Pedido (com Itens Embarcados)
```json
{
  "_id": ObjectId("..."),
  "clienteId": ObjectId("..."),
  "dataPedido": ISODate("..."),
  "itens": [
    {
      "_id": ObjectId("..."),
      "nome": "Notebook Dell",
      "preco": 2500.00,
      "quantidade": 1
    }
  ]
}
```

<COMPARISON>
**MongoDB vs SQL:**
- **MongoDB**: Documentos embarcados eliminam JOINs
- **SQL**: Tabelas relacionadas requerem chaves estrangeiras
- **MongoDB**: Schema flexível permite evolução sem migração
- **SQL**: Schema rígido garante consistência estrutural
</COMPARISON>

## 🔍 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/info` | Informações da API |
| POST | `/api/clientes` | Criar cliente |
| GET | `/api/clientes` | Listar clientes |
| GET | `/api/clientes/{id}` | Buscar cliente |
| POST | `/api/pedidos` | Criar pedido |
| GET | `/api/pedidos` | Listar pedidos |
| GET | `/api/pedidos/{id}` | Buscar pedido |
| GET | `/api/clientes/{id}/pedidos` | Pedidos de um cliente |

## 💡 Características Específicas do MongoDB

### ObjectId
```csharp
[BsonId]
[BsonRepresentation(BsonType.ObjectId)]
public string Id { get; set; } = string.Empty;
```

### Documentos Embarcados
```csharp
public List<Item> Itens { get; set; } = new();
```

### Campos Calculados
```csharp
[BsonIgnore]
public decimal Total => Itens.Sum(i => i.Preco * i.Quantidade);
```

## 🎯 Exercícios Práticos

1. **Criar dados de teste**:
   ```bash
   # Use o arquivo PedidosApi.http no VS Code
   ```

2. **Explorar no MongoDB Shell**:
   ```bash
   make mongo-shell
   use pedidos
   db.clientes.find().pretty()
   db.pedidos.find().pretty()
   ```

3. **Criar índices**:
   ```bash
   make mongo-create-indexes
   ```

<PRACTICAL-APPLICATION>
**Cenários Reais de Uso:**
- E-commerce com catálogos de produtos variáveis
- Sistemas de CMS com conteúdo flexível
- APIs REST que precisam de alta performance
- Aplicações que requerem escalabilidade horizontal
</PRACTICAL-APPLICATION>

## 🔧 Comandos Úteis

```bash
# Gerenciamento de serviços
make up          # Subir todos os serviços
make down        # Parar serviços
make logs        # Ver logs
make clean       # Limpar tudo

# Desenvolvimento
make dev-up      # Só MongoDB
make dev-api     # API local
make dev-watch   # Hot reload

# MongoDB
make mongo-shell # Acessar shell
make mongo-stats # Estatísticas
```

## 📈 Vantagens do MongoDB Demonstradas

<TRADE-OFFS>
**Vantagens:**
- Schema flexível para dados evolutivos
- Documentos embarcados eliminam JOINs
- Escalabilidade horizontal nativa
- Alto desempenho para leituras
- Indexação avançada

**Considerações:**
- Duplicação de dados vs normalização
- Transações limitadas entre collections
- Curva de aprendizado para modelagem
- Queries complexas podem ser mais difíceis
</TRADE-OFFS>

## 🔗 Acessos

- **API**: http://localhost:5021
- **Swagger**: http://localhost:5021/swagger
- **MongoDB**: localhost:27017 (admin/password123)

## 📚 Próximos Passos

- Explorar agregações no MongoDB (Aula 030)
- Implementar indexação avançada
- Estudar transações em MongoDB
- Comparar com Redis (Aula 040)
