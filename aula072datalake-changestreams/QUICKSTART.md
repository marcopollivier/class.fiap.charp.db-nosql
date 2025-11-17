# Quick Start - Data Lake com Change Streams

## Pré-requisitos

- Docker e Docker Compose
- .NET 9
- Make (opcional, mas recomendado)

## Passos para Execução

### 1. Subir o MongoDB Replica Set

```bash
# Navegar para o diretório do projeto
cd aula072datalake-changestreams

# Subir MongoDB com Replica Set configurado
make up

# OU manualmente:
docker-compose up -d
sleep 10
make init-replica
```

### 2. Inserir Dados de Exemplo

```bash
# Inserir dados de exemplo
make sample-data

# OU manualmente:
docker exec mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo --file /docker-entrypoint-initdb.d/sample-data.js
```

### 3. Executar a Aplicação Data Lake

```bash
# Terminal 1: Executar o processor
make run

# OU manualmente:
cd DataLakeProcessor
dotnet run
```

### 4. Testar Change Streams

```bash
# Terminal 2: Executar comandos de teste
docker exec -it mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo --file /docker-entrypoint-initdb.d/test-datalake.js

# OU executar comandos individuais:
docker exec -it mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo
```

#### Comandos para Testar:

```javascript
// Inserir novo pedido (verá o evento no processor)
db.pedidos.insertOne({
  clienteId: ObjectId("6564f1a2b8c9d4e5f6789abc"),
  dataPedido: new Date(),
  itens: [{
    itemId: ObjectId("6564f1a2b8c9d4e5f6789ac0"),
    nome: "Notebook Dell",
    preco: 2500.00,
    quantidade: 1
  }],
  total: 2500.00
});

// Verificar eventos no Data Lake
db.datalake.find().sort({timestamp: -1}).limit(3).pretty();
```

### 5. Verificar Resultados

```bash
# Verificar estatísticas do Data Lake
make check-datalake

# OU consultar diretamente:
docker exec mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo --eval "
  print('Eventos no Data Lake:', db.datalake.countDocuments());
  db.datalake.find().sort({timestamp: -1}).limit(3).forEach(doc => {
    print(JSON.stringify(doc, null, 2));
  });
"
```

## Estrutura dos Dados

### Dados Operacionais (Input)

- **clientes**: { nome, email }
- **itens**: { nome, preco, quantidade }
- **pedidos**: { clienteId, dataPedido, itens[], total }

### Data Lake (Output)

- **datalake**: Eventos denormalizados com dados completos

### Exemplo de Evento Processado

```json
{
  "_id": ObjectId("..."),
  "evento": "pedido_insert",
  "timestamp": ISODate("2024-11-17T..."),
  "operacao": "insert",
  "colecao": "pedidos",
  "documentId": ObjectId("..."),
  "pedido": {
    "id": ObjectId("..."),
    "total": 2500.00,
    "dataPedido": ISODate("..."),
    "cliente": {
      "nome": "João Silva",
      "email": "joao.silva@email.com"
    },
    "itens": [
      {
        "itemId": ObjectId("..."),
        "nome": "Notebook Dell",
        "preco": 2500.00,
        "quantidade": 1
      }
    ],
    "quantidadeItens": 1,
    "valorMedio": 2500.00
  },
  "metadados": {
    "origem": "change_stream",
    "versao": "1.0",
    "processadoEm": ISODate("..."),
    "hostname": "MacBook-Pro-de-Marco"
  }
}
```

## Observações Importantes

### Durante a Execução:

1. **Logs em Tempo Real**: O processor mostrará cada evento processado
2. **Denormalização**: Pedidos incluem dados completos do cliente
3. **Eventos Simples**: Clientes e itens geram eventos básicos
4. **Estatísticas**: Verifique periodicamente com `make check-datalake`

### Para Parar:

- **Processor**: Pressione `Ctrl+C` no terminal da aplicação
- **MongoDB**: Execute `make down` ou `docker-compose down`

## Solução de Problemas

### Erro "Replica Set não configurado"

```bash
# Verificar status do replica set
docker exec mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin --eval "rs.status()"

# Reconfigurar se necessário
make init-replica
```

### Erro de Conexão

```bash
# Verificar se containers estão rodando
docker-compose ps

# Verificar logs
docker-compose logs mongodb-primary
```

### Processor não recebe eventos

1. Verificar se o MongoDB é Replica Set (não standalone)
2. Verificar se há dados nas collections
3. Verificar logs da aplicação para erros de conexão

## Conceitos Demonstrados

✅ **Change Streams**: Monitoramento em tempo real  
✅ **Data Lake**: Armazenamento denormalizado  
✅ **Denormalização**: Junção de dados relacionados  
✅ **Event Processing**: Processamento assíncrono  
✅ **Replica Set**: Configuração necessária para Change Streams  
✅ **Real-time Analytics**: Consultas em dados atualizados  

---

💡 **Dica**: Execute o processor primeiro e depois insira dados para ver os Change Streams em ação!