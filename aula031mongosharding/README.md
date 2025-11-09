# Aula 031 - MongoDB Avançado: Sharding e Escalabilidade Horizontal

> **Objetivo**: Compreender conceitos de sharding no MongoDB através de um cluster com múltiplos shards, permitindo distribuição de dados baseada na inicial do nome do cliente.

## 📋 Objetivo

Demonstrar conceitos de sharding no MongoDB através de:

- **Sharding**: Distribuição horizontal de dados entre múltiplos servidores
- **Shard Key**: Estratégia de particionamento baseada na inicial do nome
- **Config Servers**: Servidores de metadados do cluster
- **Mongos Router**: Roteamento de consultas entre shards
- **Distribuição Automática**: Balanceamento de dados entre shards
- **Escalabilidade Horizontal**: Crescimento do sistema através de novos shards

## 🏗️ Arquitetura

### Diagrama da Arquitetura Sharded

```mermaid
graph TB
    subgraph "Cliente Application"
        APP[".NET Application<br/>PedidosApiSimples"]
    end
    
    subgraph "Mongos Router Layer"
        MONGOS["🔀 Mongos Router<br/>localhost:27000<br/><br/>Roteia consultas<br/>Agrega resultados"]
    end
    
    subgraph "Config Servers (Replica Set: configrs)"
        CONFIG1["📋 Config Server 1<br/>localhost:27100<br/>configsvr1"]
        CONFIG2["📋 Config Server 2<br/>localhost:27101<br/>configsvr2"]  
        CONFIG3["📋 Config Server 3<br/>localhost:27102<br/>configsvr3"]
    end
    
    subgraph "Shard 1 (A-H)"
        SHARD1["💾 Shard 1<br/>localhost:27201<br/>shard1srv<br/><br/>Clientes: A-H<br/>Ana, Bruno, Carlos..."]
    end
    
    subgraph "Shard 2 (I-P)"  
        SHARD2["💾 Shard 2<br/>localhost:27202<br/>shard2srv<br/><br/>Clientes: I-P<br/>Igor, Julia, Maria..."]
    end
    
    subgraph "Shard 3 (Q-Z)"
        SHARD3["💾 Shard 3<br/>localhost:27203<br/>shard3srv<br/><br/>Clientes: Q-Z<br/>Roberto, Sofia, Zeca..."]
    end
    
    %% Connections
    APP --> MONGOS
    MONGOS --> CONFIG1
    MONGOS --> CONFIG2  
    MONGOS --> CONFIG3
    MONGOS --> SHARD1
    MONGOS --> SHARD2
    MONGOS --> SHARD3
    
    %% Config Server Replica Set
    CONFIG1 -.-> CONFIG2
    CONFIG2 -.-> CONFIG3
    CONFIG3 -.-> CONFIG1
    
    %% Data Flow Examples
    APP -.->|"Query: nome='Ana'"| MONGOS
    MONGOS -.->|"Target Query"| SHARD1
    
    APP -.->|"Query: getAll()"| MONGOS
    MONGOS -.->|"Scatter-Gather"| SHARD1
    MONGOS -.->|"Scatter-Gather"| SHARD2
    MONGOS -.->|"Scatter-Gather"| SHARD3
    
    %% Styling
    classDef configClass fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef shardClass fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef routerClass fill:#fff3e0,stroke:#e65100,stroke-width:2px
    classDef appClass fill:#e8f5e8,stroke:#2e7d32,stroke-width:2px
    
    class CONFIG1,CONFIG2,CONFIG3 configClass
    class SHARD1,SHARD2,SHARD3 shardClass
    class MONGOS routerClass
    class APP appClass
```

### MongoDB Sharded Cluster

#### Config Servers (Metadados)

- **Config Server 1**: localhost:27100 (configsvr1)
- **Config Server 2**: localhost:27101 (configsvr2)
- **Config Server 3**: localhost:27102 (configsvr3)

#### Shard Servers (Dados)

- **Shard 1**: localhost:27201 (shard1srv) - Clientes A-H
- **Shard 2**: localhost:27202 (shard2srv) - Clientes I-P  
- **Shard 3**: localhost:27203 (shard3srv) - Clientes Q-Z

#### Mongos Router

- **Router**: localhost:27000 (mongos)

### Sharding vs Replica Set vs Instância Única

```mermaid
graph LR
    subgraph "Instância Única"
        SINGLE["MongoDB<br/>📊 1 servidor<br/>❌ Escalabilidade<br/>✅ Simplicidade"]
    end
    
    subgraph "Replica Set (aula030)"
        PRIMARY["Primary<br/>✍️ Lê/Escreve"]
        SECONDARY1["Secondary 1<br/>👁️ Lê apenas"]
        SECONDARY2["Secondary 2<br/>👁️ Lê apenas"]
        PRIMARY -.->|replicação| SECONDARY1
        PRIMARY -.->|replicação| SECONDARY2
    end
    
    subgraph "Sharded Cluster (aula031)"
        MONGOS_FLOW["Mongos Router<br/>🔀 Roteia consultas"]
        SHARD1_FLOW["Shard 1<br/>💾 Dados A-H"]
        SHARD2_FLOW["Shard 2<br/>💾 Dados I-P"]
        SHARD3_FLOW["Shard 3<br/>💾 Dados Q-Z"]
        
        MONGOS_FLOW --> SHARD1_FLOW
        MONGOS_FLOW --> SHARD2_FLOW
        MONGOS_FLOW --> SHARD3_FLOW
    end
    
    CLIENT[Cliente] --> SINGLE
    CLIENT --> PRIMARY
    CLIENT --> MONGOS_FLOW
```

### Como Funciona o Roteamento de Consultas

```mermaid
sequenceDiagram
    participant App as .NET Application
    participant Mongos as Mongos Router
    participant Config as Config Servers
    participant S1 as Shard 1 (A-H)
    participant S2 as Shard 2 (I-P)
    participant S3 as Shard 3 (Q-Z)
    
    Note over App,S3: Consulta Específica (Target Query)
    App->>Mongos: db.clientes.find({nome: "Ana"})
    Mongos->>Config: Onde está primeiraLetra="A"?
    Config-->>Mongos: Shard 1 (A-H)
    Mongos->>S1: find({nome: "Ana"})
    S1-->>Mongos: Resultado: Ana Silva
    Mongos-->>App: Retorna resultado
    
    Note over App,S3: Consulta Geral (Scatter-Gather)
    App->>Mongos: db.clientes.find({})
    Mongos->>Config: Quais shards têm dados?
    Config-->>Mongos: Todos os shards
    
    par Consulta paralela
        Mongos->>S1: find({}) - buscar A-H
        Mongos->>S2: find({}) - buscar I-P
        Mongos->>S3: find({}) - buscar Q-Z
    end
    
    par Resposta paralela
        S1-->>Mongos: Ana, Bruno, Carlos...
        S2-->>Mongos: Igor, Julia, Maria...
        S3-->>Mongos: Roberto, Sofia, Zeca...
    end
    
    Mongos-->>App: Agrega e retorna todos os resultados
```

```bash
# Instância única: Limitada a um servidor
MongoDB (single) → ❌ Escalabilidade horizontal
                   ❌ Distribuição de carga
                   ✅ Simplicidade

# Replica Set: Alta disponibilidade
Primary + Secondaries → ✅ Alta disponibilidade
                       ✅ Tolerância a falhas  
                       ❌ Escalabilidade horizontal

# Sharded Cluster: Escalabilidade + Distribuição
Mongos Router    → ✅ Escalabilidade horizontal
Shard1 + Shard2  → ✅ Distribuição de dados
+ Shard3         → ✅ Balanceamento automático
Config Servers   → ✅ Metadados distribuídos
```

### Estratégia de Sharding: Por Inicial do Nome

```mermaid
graph TD
    subgraph "Distribuição por Inicial do Nome"
        INPUT["👤 Cliente<br/>Nome: 'Ana Silva'"]
        
        EXTRACT["🔤 Extrair primeira letra<br/>nome[0] = 'A'"]
        
        DECISION{"🎯 Qual Shard?<br/>Baseado na letra"}
        
        SHARD1_RANGE["📂 Shard 1<br/>A, B, C, D, E, F, G, H<br/><br/>Exemplos:<br/>• Ana Silva<br/>• Bruno Costa<br/>• Carlos Oliveira<br/>• Helena Santos"]
        
        SHARD2_RANGE["📂 Shard 2<br/>I, J, K, L, M, N, O, P<br/><br/>Exemplos:<br/>• Igor Lima<br/>• Julia Ferreira<br/>• Maria Souza<br/>• Pedro Alves"]
        
        SHARD3_RANGE["📂 Shard 3<br/>Q, R, S, T, U, V, W, X, Y, Z<br/><br/>Exemplos:<br/>• Roberto Machado<br/>• Sofia Rodrigues<br/>• Thiago Mendes<br/>• Zeca Pagodinho"]
    end
    
    INPUT --> EXTRACT
    EXTRACT --> DECISION
    DECISION -->|"A-H"| SHARD1_RANGE
    DECISION -->|"I-P"| SHARD2_RANGE
    DECISION -->|"Q-Z"| SHARD3_RANGE
    
    classDef shard1 fill:#ffcdd2,stroke:#c62828
    classDef shard2 fill:#c8e6c9,stroke:#2e7d32
    classDef shard3 fill:#bbdefb,stroke:#1565c0
    classDef process fill:#fff9c4,stroke:#f57f17
    
    class SHARD1_RANGE shard1
    class SHARD2_RANGE shard2
    class SHARD3_RANGE shard3
    class EXTRACT,DECISION process
```

```csharp
// DISTRIBUIÇÃO POR INICIAL DO NOME
Shard 1 (A-H): "Ana", "Bruno", "Carlos", "Helena"
Shard 2 (I-P): "Igor", "Julia", "Maria", "Pedro"  
Shard 3 (Q-Z): "Roberto", "Sofia", "Thiago", "Zeca"

// SHARD KEY: Primeira letra do nome
{ "nome": "hashed" } // MongoDB calcula hash da primeira letra
// OU
{ "primeiraLetra": 1 } // Campo calculado explícito
```

## 🚀 Execução

### 1. Inicializar o Cluster

```bash
# Subir todos os containers
make up

# Aguardar inicialização (30-60 segundos)
make logs

# Configurar sharding
make setup-sharding
```

### 2. Testar Distribuição

```bash
# Executar exemplo .NET
cd PedidosApiSimples
dotnet run
```

### 3. Verificar Distribuição

```bash
# Conectar ao mongos
make mongo-shell

# Verificar status do cluster
sh.status()

# Verificar distribuição de dados
use pedidos
db.clientes.getShardDistribution()
```

## 📊 Comandos Úteis

### Configuração do Sharding

```javascript
// Habilitar sharding no banco
sh.enableSharding("pedidos")

// Criar índice na shard key
db.clientes.createIndex({ "primeiraLetra": 1 })

// Configurar sharding na coleção
sh.shardCollection("pedidos.clientes", { "primeiraLetra": 1 })

// Verificar status
sh.status()
```

### Monitoramento

```javascript
// Ver distribuição de chunks
db.printShardingStatus()

// Estatísticas por shard
db.clientes.getShardDistribution()

// Verificar balanceamento
sh.getBalancerState()
```

## 🔍 Comparação: Sem vs Com Sharding

### Sem Sharding (Instância Única)

```csharp
// TODOS os clientes no mesmo servidor
await collection.InsertOneAsync(cliente); // Sempre no mesmo lugar
var clientes = await collection.Find(x => true).ToListAsync(); // Uma consulta
```

### Com Sharding (Cluster Distribuído)

```csharp
// Clientes distribuídos automaticamente
await collection.InsertOneAsync(cliente); // Mongos roteia para shard correto
var clientes = await collection.Find(x => true).ToListAsync(); // Consulta todos os shards
```

## 🎯 Conceitos Fundamentais

### Shard Key

- **Definição**: Campo que determina como os dados são distribuídos
- **Estratégias**: Range, Hash, Zone-based
- **Escolha**: Deve distribuir uniformemente e permitir consultas eficientes

### Chunks

- **Definição**: Blocos lógicos de dados dentro de um range da shard key
- **Tamanho**: Padrão 64MB, configurável
- **Migração**: Movidos automaticamente entre shards para balanceamento

### Balanceamento

- **Automático**: MongoDB move chunks quando necessário
- **Manual**: Comandos específicos para forçar migração
- **Configurável**: Pode ser habilitado/desabilitado

## 🔧 Arquivos de Configuração

- `docker-compose.yml`: Definição completa do cluster sharded
- `configure-sharding.sh`: Script de configuração automática
- `Makefile`: Comandos para gerenciar o cluster
- `PedidosApiSimples/`: Aplicação .NET demonstrando uso

## 📚 Recursos Adicionais

- [MongoDB Sharding](https://docs.mongodb.com/manual/sharding/)
- [Shard Key Selection](https://docs.mongodb.com/manual/core/sharding-shard-key/)
- [Balancer](https://docs.mongodb.com/manual/core/sharding-balancer-administration/)

## 🎓 Próximos Passos

1. **Aula 040**: Redis para cache e sessões
2. **Aula 050**: DynamoDB - NoSQL da AWS
3. **Comparação**: Quando usar cada tipo de banco NoSQL

---

**Nota**: Este setup é para aprendizado. Em produção, use replica sets em cada shard para alta disponibilidade.
