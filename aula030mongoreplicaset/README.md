# Aula 030 - MongoDB Avançado: Replica Sets e Transações

> **Objetivo**: Compreender conceitos avançados do MongoDB através de replica sets, transações e alta disponibilidade em um setup automatizado prático.

## 📋 Objetivo

Demonstrar conceitos avançados do MongoDB através de:

- **Replica Sets**: Configuração de alta disponibilidade com 3 instâncias MongoDB
- **Transações**: ACID compliance entre múltiplos documentos  
- **Persistência Tradicional vs Atômica**: Comparação entre operações CRUD normais e transações
- **Failover**: Eleição automática de novo primary
- **Alta Disponibilidade**: Sistema funciona mesmo com falhas

## 🏗️ Arquitetura

### MongoDB Replica Set

- **Primary**: localhost:27017 (mongo-primary)
- **Secondary 1**: localhost:27018 (mongo-secondary1)  
- **Secondary 2**: localhost:27019 (mongo-secondary2)

### Replica Set vs Instância Única

```bash
# Instância única: SEM transações
MongoDB (single) → ❌ Transações multi-documento
                   ❌ Alta disponibilidade  
                   ❌ Tolerância a falhas

# Replica Set: COM transações
Primary    → ✅ Lê e escreve
Secondary1 → ✅ Cópia dos dados
Secondary2 → ✅ Backup automático
           → ✅ Transações ACID
           → ✅ Failover automático
```

### Transações: Individual vs Atômica

```csharp
// INDIVIDUAL: Cada operação é isolada
await collection.InsertOneAsync(cliente1);  // ✅ Sempre atômica
await collection.InsertOneAsync(cliente2);  // ✅ Sempre atômica
// Problema: Se falhar no meio, fica inconsistente!

// TRANSAÇÃO: Múltiplas operações atômicas
using var session = await client.StartSessionAsync();
session.StartTransaction();
await collection.InsertOneAsync(session, cliente1);
await collection.InsertOneAsync(session, cliente2);
await session.CommitTransactionAsync(); // ✅ Tudo ou nada!
```

## � Estrutura do Projeto

```
aula030mongoreplicaset/
├── docker-compose.yml          # 3 instâncias MongoDB em replica set
├── configure-replica-set.sh    # Setup automatizado
├── Makefile                   # Comandos úteis
├── PedidosApiSimples/         # Aplicação .NET demonstrativa
│   ├── Cliente.cs            # Modelo para MongoDB
│   ├── Program.cs            # Exemplos práticos
│   └── PedidosApiSimples.csproj
└── README.md                 # Este arquivo
```

## 🚀 Como Executar

### Opção 1: Setup Automático (Recomendado)

```bash
# Um comando faz tudo: containers + replica set + dados
make restart
```

### Opção 2: Passo a Passo

```bash

make up

make data    # Ver dados# 1. Derrubar tudo e limpar dados

# 2. Verificar replica set

make status```docker compose down -v



# 1. Subir containers
docker compose up -d

# 2. Aguardar inicialização (30 segundos)
sleep 30

# 3. Verificar se replica set está funcionando
make status

# 4. Ver dados de exemplo
make data
```

## 🔧 Comandos Disponíveis

```bash
make restart    # Derrubar tudo + recriar limpo
make up         # Subir ambiente
make down       # Parar containers  
make clean      # Limpar volumes
make status     # Status do replica set
make data       # Ver dados inseridos
make compass    # Instruções MongoDB Compass
```

## 🔗 Conectar no MongoDB Compass

**URI de Conexão:**

```
mongodb://localhost:27017/?directConnection=true
```

**Para explorar:**

- Database: `exemploSimples`
- Collection: `clientes`

## 🧪 Testar Conceitos

### 1. Executar Aplicação .NET

```bash
cd PedidosApiSimples
dotnet run
```

**O que a aplicação demonstra:**

- Persistência individual (sempre funciona)
- Persistência com transação (só funciona em replica set)
- Diferenças entre os dois approaches

### 2. Simular Falha do Primary

```bash
# Derrubar o primary
docker stop mongo-primary

# Ver eleição do novo primary
make status

# Primary original volta como secondary
docker start mongo-primary
```

### 3. Verificar os Dados

**No MongoDB Compass:**

1. Conecte com: `mongodb://localhost:27017/?directConnection=true`
2. Navegue até: `exemploSimples` > `clientes`
3. Visualize os documentos inseridos

**Via Comando:**

```bash
# Ver documentos na coleção
docker exec mongo-primary mongosh exemploSimples --eval 'db.clientes.find().pretty()'

# Verificar status do replica set
docker exec mongo-primary mongosh --eval 'rs.status()'
```

## Conceitos Demonstrados

### 1. Persistência Tradicional

- Operações CRUD individuais (InsertOneAsync, UpdateOneAsync, etc.)
- Sem garantias transacionais entre operações
- Pode resultar em inconsistências em caso de falha

### 2. Persistência Atômica (Transações)

- Múltiplas operações agrupadas em uma transação
- Garantia ACID: tudo sucede ou tudo falha
- Requer replica set (não funciona em instância única)

### 3. Replica Sets

- **Alta Disponibilidade**: Se primary falhar, secondary assume
- **Escalabilidade de Leitura**: Reads podem ser distribuídos
- **Tolerância a Falhas**: Sistema funciona mesmo com 1 instância down

## ⚡ Troubleshooting

**Problema: "not primary"**

- **Causa**: Tentou inserir dados antes da eleição terminar
- **Solução**: Aguardar ~15 segundos após `make up`

**Problema: "no write concern"**

- **Causa**: Replica set não está configurado
- **Solução**: Executar `./configure-replica-set.sh`

**Problema: MongoDB Compass não conecta**

- **Solução**: Usar URI com `directConnection=true`

## 💡 Insights Importantes

1. **Operações únicas sempre são atômicas** no MongoDB
2. **Transações só funcionam** em replica set ou sharded cluster  
3. **Replica sets** são essenciais para produção
4. **Failover automático** garante disponibilidade
5. **MongoDB não é só "NoSQL"** - tem ACID quando necessário

## 🎯 Próximos Passos

- Experimente derrubar o primary e ver a eleição de novo primary
- Teste transações que falham no meio e observe o rollback
- Compare performance de reads entre primary e secondaries

**Próximas Aulas:**

- [Aula 4.0: Redis Fundamentals](../aula040redis/)
- [Aula 5.0: DynamoDB Basics](../aula050dynamodbfundamentos/)
- [Comparação NoSQL](../aula070comparacao/)
