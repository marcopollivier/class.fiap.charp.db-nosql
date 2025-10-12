# Exemplos de Teste - NoSQL vs SQL

## Subir os bancos de dados
```bash
docker-compose up -d
```

## Testar MongoDB (NoSQL)

### Criar Cliente
```bash
curl -X POST "http://localhost:5000/api/mongo/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "email": "joao@email.com"
  }'
```

### Consultar Cliente
```bash
curl "http://localhost:5000/api/mongo/clientes/{id}"
```

### Criar Pedido
```bash
curl -X POST "http://localhost:5000/api/mongo/pedidos" \
  -H "Content-Type: application/json" \
  -d '{
    "clienteId": "{clienteId}",
    "itens": [
      {
        "nome": "Produto A",
        "preco": 10.50,
        "quantidade": 2
      },
      {
        "nome": "Produto B",
        "preco": 25.00,
        "quantidade": 1
      }
    ]
  }'
```

### Consultar Pedido
```bash
curl "http://localhost:5000/api/mongo/pedidos/{id}"
```

## Testar SQL Server (SQL)

### Criar Cliente
```bash
curl -X POST "http://localhost:5000/api/sql/clientes" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Maria Santos",
    "email": "maria@email.com"
  }'
```

### Consultar Cliente
```bash
curl "http://localhost:5000/api/sql/clientes/1"
```

### Criar Pedido
```bash
curl -X POST "http://localhost:5000/api/sql/pedidos" \
  -H "Content-Type: application/json" \
  -d '{
    "clienteId": 1,
    "itens": [
      {
        "nome": "Produto C",
        "preco": 15.75,
        "quantidade": 3
      }
    ]
  }'
```

### Consultar Pedido
```bash
curl "http://localhost:5000/api/sql/pedidos/1"
```

## Principais Diferenças Observadas

### MongoDB (NoSQL)
- IDs são strings (ObjectId)
- Estrutura de documento aninhado (itens dentro do pedido)
- Flexibilidade de schema
- Consulta simples por documento

### SQL Server (SQL)
- IDs são inteiros auto-incrementais
- Estrutura relacional (tabelas separadas)
- Schema rígido e definido
- Necessita de JOINs para relacionamentos
