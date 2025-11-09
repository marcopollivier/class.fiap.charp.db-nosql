# Aula 3.0: MongoDB - Replica Sets# Aula 030 - MongoDB Replica Set# Aula 030 - MongoDB Avançado: Replica Sets e Persistência Atômica

> **Objetivo**: Compreender conceitos avançados do MongoDB através de replica sets, transações e alta disponibilidade em um setup automatizado prático.## 📋 Objetivo## 📋 Objetivo

## 🎯 Por que Este Setup?Demonstrar MongoDB Replica Set com 3 instâncias:Demonstrar conceitos avançados do MongoDB através de

"O que é um replica set?" e "Como MongoDB garante alta disponibilidade?"- **Primary**: localhost:27017

Este laboratório demonstra conceitos fundamentais de produção:- **Secondary 1**: localhost:27018- **Replica Sets**: Configuração de alta disponibilidade com 3 instâncias MongoDB

- **Replica Set**: 1 primary + 2 secondaries

- **Transações**: ACID compliance entre múltiplos documentos- **Secondary 2**: localhost:27019- **Persistência Tradicional vs Atômica**: Comparação entre operações CRUD normais e transações

- **Failover**: Eleição automática de novo primary

- **Alta Disponibilidade**: Sistema funciona mesmo com falhas## 🚀 Como Testar do Zero## 🏗️ Arquitetura

## 📊 Conceitos que Você Vai Aprender```bash### MongoDB Replica Set

### Replica Set vs Instância Única# Restart completo (limpa e recria tudo)

```bashmake restart- **Primary**: localhost:27017 (mongo-primary)

# Instância única: SEM transações

MongoDB (single) → ❌ Transações multi-documento- **Secondary 1**: localhost:27018 (mongo-secondary1)

                   ❌ Alta disponibilidade  

                   ❌ Tolerância a falhas# Verificar se funcionou- **Secondary 2**: localhost:27019 (mongo-secondary2)



# Replica Set: COM transaçõesmake status

Primary    → ✅ Lê e escreve

Secondary1 → ✅ Cópia dos dadosmake data### Estrutura do Projeto

Secondary2 → ✅ Backup automático

           → ✅ Transações ACID```

           → ✅ Failover automático

``````

### Transações: Individual vs Atômica## 🔗 MongoDB Compassaula030mongoavancado/

```csharp├── docker-compose.yml          # 3 instâncias MongoDB em replica set

// INDIVIDUAL: Cada operação é isolada

await collection.InsertOneAsync(cliente1);  // ✅ Sempre atômica**URI:** `mongodb://localhost:27017/?directConnection=true`├── ExemploSimples/             # Projeto .NET 9 demonstrativo

await collection.InsertOneAsync(cliente2);  // ✅ Sempre atômica

// Problema: Se falhar no meio, fica inconsistente!│   ├── Cliente.cs              # Modelo de dados



// TRANSAÇÃO: Múltiplas operações atômicas- Database: `exemploSimples`│   ├── Program.cs              # Exemplos de persistência

using var session = await client.StartSessionAsync();

session.StartTransaction();- Collection: `clientes`│   └── ExemploSimples.csproj   # Configuração do projeto

await collection.InsertOneAsync(session, cliente1);

await collection.InsertOneAsync(session, cliente2);└── README.md                   # Este arquivo

await session.CommitTransactionAsync(); // ✅ Tudo ou nada!

```## 🔧 Outros Comandos```



## 🚀 Como Executar



### Opção 1: Setup Automático (Recomendado)```bash## 🚀 Como Testar do Zero

```bash

# Um comando faz tudo: containers + replica set + dadosmake up      # Subir ambiente

make restart

```make down    # Parar containers### Opção 1: Comandos Manuais



### Opção 2: Passo a Passomake clean   # Remover tudo

```bash

# 1. Subir containersmake status  # Ver replica set```bash

make up

make data    # Ver dados# 1. Derrubar tudo e limpar dados

# 2. Verificar replica set

make status```docker compose down -v



# 3. Ver dados inseridos# 2. Subir ambiente limpo

make datadocker compose up -d

```

# 3. Aguardar 30 segundos para inicialização automática

## 🔗 Conectar no MongoDB Compasssleep 30

**URI de Conexão:**# 4. Verificar se funcionou

```docker exec mongo-primary mongosh --eval 'rs.status()'

mongodb://localhost:27017/?directConnection=true```

```

### Opção 2: Usando Makefile (Recomendado)

**Para explorar:**

- Database: `pedidos````bash

- Collection: `clientes`# Restart completo (limpa tudo e recria)

make restart

## 🧪 Testar Conceitos

# Ver status

### 1. Executar Aplicação .NETmake status

```bash

cd PedidosApiSimples# Ver dados de exemplo

dotnet runmake data

```

# Instruções MongoDB Compass

**O que a aplicação demonstra:**make compass

- Persistência individual (sempre funciona)```

- Persistência com transação (só funciona em replica set)

- Diferenças entre os dois approaches## 🚀 Como Executar (Primeira Vez)

### 2. Simular Falha do Primary### 1. Subir o Ambiente

```bash

# Derrubar o primary```bash

docker stop mongo-primary# Sobe os 3 containers MongoDB

docker compose up -d

# Ver eleição do novo primary

make status# Verifica se os containers estão rodando

docker compose ps

# Primary original volta como secondary```

docker start mongo-primary

```### 2. Configurar Replica Set



## 📁 Estrutura do Projeto```bash

# Configurar replica set (já configurado automaticamente)

```docker exec mongo-primary mongosh --eval 'rs.status()'

aula030mongoreplicaset/```

├── docker-compose.yml          # 3 instâncias MongoDB

├── configure-replica-set.sh    # Setup automatizado### 3. Conectar no MongoDB Compass

├── Makefile                   # Comandos úteis

├── PedidosApiSimples/         # Aplicação .NET demonstrativa**URI de Conexão (Recomendada):**

│   ├── Cliente.cs            # Modelo para MongoDB

│   ├── Program.cs            # Exemplos práticos```

│   └── PedidosApiSimples.csprojmongodb://localhost:27017/?directConnection=true

└── README.md                 # Este arquivo```

```

**Alternativa para Replica Set (Avançado):**

## 🔧 Comandos Disponíveis

```

```bashmongodb://localhost:27017,localhost:27018,localhost:27019/?replicaSet=rs0

make restart    # Derrubar tudo + recriar limpo```

make up         # Subir ambiente

make down       # Parar containers  ### 4. Executar Exemplo .NET

make clean      # Limpar volumes

make status     # Status do replica set```bash

make data       # Ver dados inseridoscd ExemploSimples

```dotnet run

```

## ⚡ Troubleshooting

## � Verificando os Dados

**Problema: "not primary"**

- **Causa**: Tentou inserir dados antes da eleição terminar### No MongoDB Compass

- **Solução**: Aguardar ~15 segundos após `make up`

1. Conecte com: `mongodb://localhost:27017/?directConnection=true`

**Problema: "no write concern"**2. Navegue até: `exemploSimples` > `clientes`

- **Causa**: Replica set não está configurado3. Visualize os documentos inseridos

- **Solução**: Executar `./configure-replica-set.sh`

### Via Comando

**Problema: MongoDB Compass não conecta**

- **Solução**: Usar URI com `directConnection=true````bash

# Ver documentos na coleção

## 🎯 Próximos Passosdocker exec mongo-primary mongosh exemploSimples --eval 'db.clientes.find().pretty()'

- [Aula 4.0: Redis Fundamentals](../aula040redis/)# Verificar status do replica set

- [Aula 5.0: DynamoDB Basics](../aula050dynamodbfundamentos/)docker exec mongo-primary mongosh --eval 'rs.status()'

- [Comparação NoSQL](../aula070comparacao/)```

## 💡 Insights Importantes## 📚 Conceitos Demonstrados

1. **Operações únicas sempre são atômicas** no MongoDB### 1. Persistência Tradicional

2. **Transações só funcionam** em replica set ou sharded cluster  

3. **Replica sets** são essenciais para produção- Operações CRUD individuais (InsertOneAsync, UpdateOneAsync, etc.)

4. **Failover automático** garante disponibilidade- Sem garantias transacionais entre operações

5. **MongoDB não é só "NoSQL"** - tem ACID quando necessário- Pode resultar em inconsistências em caso de falha

### 2. Persistência Atômica (Transações)

- Múltiplas operações agrupadas em uma transação
- Garantia ACID: tudo sucede ou tudo falha
- Requer replica set (não funciona em instância única)

### 3. Replica Sets

- **Alta Disponibilidade**: Se primary falhar, secondary assume
- **Escalabilidade de Leitura**: Reads podem ser distribuídos
- **Tolerância a Falhas**: Sistema funciona mesmo com 1 instância down

## � Comandos Úteis

```bash
# Parar ambiente
docker compose down

# Ver logs de um container específico
docker logs mongo-primary

# Conectar diretamente no MongoDB
docker exec -it mongo-primary mongosh

# Limpar volumes (dados)
docker compose down -v
```

## ⚠️ Notas Importantes

1. **Conectividade**: Use `directConnection=true` no MongoDB Compass para conexão simples
2. **Dados**: Persistidos em volumes Docker (sobrevivem a restart dos containers)
3. **Rede**: Containers se comunicam via rede Docker interna
4. **Desenvolvimento**: Configuração adequada para ambiente de desenvolvimento/estudo

## 🎯 Próximos Passos

- Experimente derrubar o primary e ver a eleição de novo primary
- Teste transações que falham no meio e observe o rollback
- Compare performance de reads entre primary e secondaries
