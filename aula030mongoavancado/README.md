# Aula 030 - MongoDB Replica Set# Aula 030 - MongoDB Avançado: Replica Sets e Persistência Atômica

## 📋 Objetivo## 📋 Objetivo

Demonstrar MongoDB Replica Set com 3 instâncias:Demonstrar conceitos avançados do MongoDB através de:

- **Primary**: localhost:27017

- **Secondary 1**: localhost:27018- **Replica Sets**: Configuração de alta disponibilidade com 3 instâncias MongoDB

- **Secondary 2**: localhost:27019- **Persistência Tradicional vs Atômica**: Comparação entre operações CRUD normais e transações

## 🚀 Como Testar do Zero## 🏗️ Arquitetura

```bash### MongoDB Replica Set

# Restart completo (limpa e recria tudo)

make restart- **Primary**: localhost:27017 (mongo-primary)

- **Secondary 1**: localhost:27018 (mongo-secondary1)

# Verificar se funcionou- **Secondary 2**: localhost:27019 (mongo-secondary2)

make status

make data### Estrutura do Projeto

```

```

## 🔗 MongoDB Compassaula030mongoavancado/

├── docker-compose.yml          # 3 instâncias MongoDB em replica set

**URI:** `mongodb://localhost:27017/?directConnection=true`├── ExemploSimples/             # Projeto .NET 9 demonstrativo

│   ├── Cliente.cs              # Modelo de dados

- Database: `exemploSimples`│   ├── Program.cs              # Exemplos de persistência

- Collection: `clientes`│   └── ExemploSimples.csproj   # Configuração do projeto

└── README.md                   # Este arquivo

## 🔧 Outros Comandos```



```bash## 🚀 Como Testar do Zero

make up      # Subir ambiente

make down    # Parar containers### Opção 1: Comandos Manuais

make clean   # Remover tudo

make status  # Ver replica set```bash

make data    # Ver dados# 1. Derrubar tudo e limpar dados

```docker compose down -v

# 2. Subir ambiente limpo
docker compose up -d

# 3. Aguardar 30 segundos para inicialização automática
sleep 30

# 4. Verificar se funcionou
docker exec mongo-primary mongosh --eval 'rs.status()'
```

### Opção 2: Usando Makefile (Recomendado)

```bash
# Restart completo (limpa tudo e recria)
make restart

# Ver status
make status

# Ver dados de exemplo
make data

# Instruções MongoDB Compass
make compass
```

## 🚀 Como Executar (Primeira Vez)

### 1. Subir o Ambiente

```bash
# Sobe os 3 containers MongoDB
docker compose up -d

# Verifica se os containers estão rodando
docker compose ps
```

### 2. Configurar Replica Set

```bash
# Configurar replica set (já configurado automaticamente)
docker exec mongo-primary mongosh --eval 'rs.status()'
```

### 3. Conectar no MongoDB Compass

**URI de Conexão (Recomendada):**

```
mongodb://localhost:27017/?directConnection=true
```

**Alternativa para Replica Set (Avançado):**

```
mongodb://localhost:27017,localhost:27018,localhost:27019/?replicaSet=rs0
```

### 4. Executar Exemplo .NET

```bash
cd ExemploSimples
dotnet run
```

## � Verificando os Dados

### No MongoDB Compass

1. Conecte com: `mongodb://localhost:27017/?directConnection=true`
2. Navegue até: `exemploSimples` > `clientes`
3. Visualize os documentos inseridos

### Via Comando

```bash
# Ver documentos na coleção
docker exec mongo-primary mongosh exemploSimples --eval 'db.clientes.find().pretty()'

# Verificar status do replica set
docker exec mongo-primary mongosh --eval 'rs.status()'
```

## 📚 Conceitos Demonstrados

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
