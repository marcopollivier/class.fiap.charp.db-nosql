# Aula 4: Redis - Fundamentos e Persistência/HA

> **Objetivo**: Compreender Redis como banco de dados em memória, seus tipos de dados únicos, padrões de cache e estratégias de alta disponibilidade.

## 🎯 O que Você Vai Aprender

- Tipos de dados Redis (strings, hashes, listas, sets, sorted sets)
- Redis como cache distribuído
- Padrões: Cache-aside, Write-through, Write-behind
- Persistência: RDB vs AOF
- Alta disponibilidade: Sentinel e Cluster
- Integração com .NET (StackExchange.Redis)

## 🚀 Como Executar

```bash
# 1. Subir a infraestrutura Redis
docker-compose up -d

# 2. Navegar para o projeto .NET
cd /caminho/para/aula040redis

# 3. Restaurar dependências
dotnet restore RedisExamplesApi.csproj

# 4. Executar a aplicação
dotnet run --project RedisExamplesApi.csproj

# 5. Acessar a API
# - Swagger UI: http://localhost:5000
# - Health Check: http://localhost:5000/health
# - Exemplos: http://localhost:5000/api/redisexamples
```

### Testando o Redis diretamente

```bash
# Conectar ao container Redis
docker exec -it aula04-redis redis-cli -a password123

# Comandos básicos para teste
SET teste "Hello Redis"
GET teste
KEYS *
```

## 📚 Tópicos Abordados

### Material Didático Disponível

O conteúdo completo da aula está organizado na pasta `doc/`:

1. **[Introdução ao Redis](doc/01-introducao-redis.md)** - Conceitos fundamentais e características
2. **[Tipos de Dados](doc/02-tipos-dados.md)** - Strings, Hashes, Lists, Sets, Sorted Sets
3. **[Cache Distribuído](doc/03-cache-distribuido.md)** - Padrões de cache e implementação prática
4. **[Persistência](doc/04-persistencia.md)** - RDB vs AOF, estratégias de backup
5. **[Casos de Uso Avançados](doc/05-casos-uso-avancados.md)** - Rate limiting, leaderboards, filas
6. **[Resumo e Melhores Práticas](doc/06-resumo-melhores-praticas.md)** - Produção e monitoramento

### Exemplos Práticos

- **API .NET completa** com exemplos funcionais de todos os conceitos
- **Controller demonstrativo** em `RedisExamplesController.cs`
- **Configuração Docker** pronta para uso
- **Health checks** e monitoramento

## 🔗 Links Relacionados

- [Aula 3.0: MongoDB Avançado](../aula030mongoavancado/)
- [Aula 5.0: DynamoDB Fundamentos](../aula050dynamodbfundamentos/)
