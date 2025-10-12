# Sistema de Pedidos - NoSQL vs SQL

Sistema simples para demonstrar as diferenças entre bancos NoSQL (MongoDB) e SQL (SQL Server) usando .NET.

## Estrutura do Projeto

- **Entidades**: Cliente, Pedido, Item
- **Repositories**: MongoRepository e SqlRepository
- **Controllers**: MongoController (`/api/mongo`) e SqlController (`/api/sql`)

## Como Executar

1. **Subir os bancos de dados:**
```bash
docker-compose up -d
```

2. **Executar a API:**
```bash
cd PedidosApi
dotnet run
```

3. **Executar lint (formatação e análise):**
```bash
./lint.sh
```

4. **Acessar Swagger:**
- http://localhost:5000/swagger

## Endpoints Disponíveis

### MongoDB (NoSQL)
- `POST /api/mongo/clientes` - Criar cliente
- `GET /api/mongo/clientes/{id}` - Consultar cliente
- `POST /api/mongo/pedidos` - Criar pedido
- `GET /api/mongo/pedidos/{id}` - Consultar pedido

### SQL Server (SQL)
- `POST /api/sql/clientes` - Criar cliente
- `GET /api/sql/clientes/{id}` - Consultar cliente
- `POST /api/sql/pedidos` - Criar pedido
- `GET /api/sql/pedidos/{id}` - Consultar pedido

## Principais Diferenças

| Aspecto | MongoDB (NoSQL) | SQL Server (SQL) |
|---------|-----------------|------------------|
| **ID** | String (ObjectId) | Integer (Auto-increment) |
| **Estrutura** | Documento aninhado | Tabelas relacionais |
| **Schema** | Flexível | Rígido e definido |
| **Relacionamentos** | Embarcado no documento | JOINs entre tabelas |
| **Consultas** | Por documento completo | Necessita JOINs |

## Exemplos de Uso

Consulte o arquivo `exemplos-teste.md` para exemplos práticos de como testar ambas as implementações.
