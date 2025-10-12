# FIAP - Pós-Graduação .NET - NoSQL Database

Este repositório contém uma série de projetos práticos para o curso de NoSQL utilizando .NET SDK 9 e C#.

## Estrutura do Curso

### Aula 010 – Introdução ao NoSQL

Fundamentos do NoSQL, modelos de dados, trade-offs em relação a bancos relacionais e integração com .NET.

**Tópicos:**

- Categorias (Documento, chave-valor, coluna larga, grafo)
- Comparação entre SQL e NoSQL
- CAP theorem
- Integração com .NET (MongoDB, Redis, DynamoDB)
- Demo comparativo SQL Server vs MongoDB

### Aula 020 – MongoDB Básico

Exploração do MongoDB, operações básicas e integração com aplicações .NET.

**Tópicos:**

- Documentos, coleções e BSON
- CRUD operations
- Índices básicos
- Integração .NET com POCOs
- Exemplo prático: CRUD de produtos

### Aula 030 – MongoDB Avançado

Modelagem avançada, processamento de dados e recursos enterprise do MongoDB.

**Tópicos:**

- Modelagem (embedded vs referenced)
- Aggregation Framework
- Transações no MongoDB
- Change Streams
- Sharding e Replica Sets

### Aula 040 – Redis Fundamentos e Persistência/HA

Redis como banco em memória, persistência, alta disponibilidade e padrões avançados.

**Tópicos:**

- Tipos de dados (strings, hashes, listas, sets, sorted sets)
- Redis como cache distribuído
- Integração .NET
- Persistência (RDB vs AOF)
- Padrões: Redlock e Leaderboards

### Aula 050 – DynamoDB Fundamentos e Modelagem

Estrutura do DynamoDB, índices e boas práticas de modelagem.

**Tópicos:**

- Chave-partição e sort key
- CRUD com .NET SDK
- Índices (GSI e LSI)
- Single table design vs múltiplas tabelas
- Hot spots e particionamento

### Aula 060 – DynamoDB Avançado

Recursos avançados, consistência, streams e padrões arquiteturais.

**Tópicos:**

- Consistência eventual vs forte
- Transações em DynamoDB
- TTL (Time To Live)
- DynamoDB Streams
- Adaptive Capacity

### Aula 070 – Comparação, Boas Práticas e Projeto Final

Consolidação de conceitos, comparação de tecnologias e projeto integrado.

**Tópicos:**

- Comparação MongoDB x Redis x DynamoDB
- Trade-offs de custo e performance
- Estratégias híbridas em .NET
- Padrões: CQRS + Event Sourcing

## Pré-requisitos

- .NET SDK 9.0
- Docker Desktop
- Visual Studio 2022 ou VS Code
- Git

## Como usar

1. Clone o repositório
2. Navegue até a pasta da aula desejada (ex: `aula010introducao`)
3. Execute `dotnet restore` para restaurar as dependências
4. Siga as instruções específicas de cada módulo

## Tecnologias Abordadas

- **Document Store**: MongoDB
- **Key-Value/Cache**: Redis
- **Key-Value/Serverless**: DynamoDB

## Estrutura de Numeração

O projeto usa numeração 010, 020, 030... para permitir subprojetos:

- `aula010introducao/` - Projeto principal
- `aula011introducao-demo/` - Demo adicional (se necessário)
- `aula012introducao-cap/` - Exemplo CAP theorem (se necessário)

## Comandos Úteis

```bash
# Restaurar dependências
dotnet restore

# Executar projeto
dotnet run

# Executar testes
dotnet test

# Subir containers Docker (dentro da pasta da aula)
docker-compose up -d
```

## Contato

Para dúvidas sobre o curso, entre em contato através dos canais oficiais da FIAP.
