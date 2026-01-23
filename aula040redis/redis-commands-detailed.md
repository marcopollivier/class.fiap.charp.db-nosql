# Comandos Redis - Aula 04

## Conectar ao Redis CLI

```bash
docker exec -it aula04-redis redis-cli -a password123
```

**Descrição**: Conecta ao container Redis usando o CLI oficial com autenticação por senha.

## Comandos básicos de exploração

```redis
KEYS *                 # Lista todas as chaves do banco atual
DBSIZE                 # Retorna o número total de chaves
INFO keyspace          # Mostra estatísticas dos bancos e chaves
```

**Descrições**:

- `KEYS *`: Lista todas as chaves existentes (cuidado em produção com muitas chaves!)
- `DBSIZE`: Conta rapidamente quantas chaves existem no banco atual
- `INFO keyspace`: Exibe informações sobre cada database e suas chaves

## Consultar cache de produtos (Strings)

```redis
GET produto:1          # Obtém o valor da chave produto:1
GET produto:2          # Obtém o valor da chave produto:2
TTL produto:1          # Mostra tempo restante até expirar (em segundos)
```

**Descrições**:

- `GET chave`: Recupera o valor de uma string armazenada
- `TTL chave`: Retorna quantos segundos faltam para a chave expirar (-1 = sem expiração, -2 = já expirou)

## Consultar contadores de visitas (Numbers)

```redis
GET produto:1:visitas  # Obtém o contador atual de visitas
GET produto:2:visitas  # Obtém o contador atual de visitas
INCR produto:1:visitas # Incrementa o contador em +1 automaticamente
```

**Descrições**:

- `GET chave`: Recupera o valor numérico como string
- `INCR chave`: Incrementa atomicamente o valor em 1 (cria a chave com valor 1 se não existir)

## Consultar dados de usuários (Hashes)

```redis
HGETALL usuario:1000   # Retorna todos os campos e valores do hash
HGET usuario:1000 nome # Retorna apenas o campo 'nome'
HGET usuario:1000 email# Retorna apenas o campo 'email'
HKEYS usuario:1000     # Lista apenas os nomes dos campos
```

**Descrições**:

- `HGETALL chave`: Retorna todos os pares campo-valor do hash
- `HGET chave campo`: Retorna o valor de um campo específico
- `HKEYS chave`: Lista apenas os nomes dos campos (sem valores)

## Consultar tags de produtos (Sets)

```redis
SMEMBERS produto:1:tags              # Lista todos os membros do set
SCARD produto:1:tags                 # Conta quantos membros tem o set
SISMEMBER produto:1:tags "eletrônicos" # Verifica se um valor existe no set
```

**Descrições**:

- `SMEMBERS chave`: Retorna todos os valores únicos do set
- `SCARD chave`: Conta quantos elementos existem no set
- `SISMEMBER chave valor`: Retorna 1 se o valor existe no set, 0 se não existe

## Consultar leaderboard (Sorted Sets)

```redis
ZRANGE leaderboard:mensal 0 -1 WITHSCORES    # Lista todos por ranking (menor para maior)
ZREVRANGE leaderboard:mensal 0 2 WITHSCORES  # Top 3 (maior para menor)
ZRANK leaderboard:mensal "JogadorPro"        # Posição do jogador (0-based)
ZSCORE leaderboard:mensal "JogadorPro"       # Pontuação do jogador
```

**Descrições**:

- `ZRANGE chave início fim WITHSCORES`: Lista elementos ordenados por score (crescente)
- `ZREVRANGE chave início fim WITHSCORES`: Lista elementos em ordem decrescente (ranking)
- `ZRANK chave membro`: Retorna a posição do membro (começando do 0)
- `ZSCORE chave membro`: Retorna a pontuação específica do membro

## Consultar tipos de dados

```redis
TYPE produto:1         # Retorna: string
TYPE produto:1:visitas # Retorna: string (números são strings no Redis)
TYPE usuario:1000      # Retorna: hash
TYPE produto:1:tags    # Retorna: set
TYPE leaderboard:mensal# Retorna: zset (sorted set)
```

**Descrição**: `TYPE chave` identifica qual estrutura de dados está armazenada na chave.

## Consultar TTL (Time To Live)

```redis
TTL produto:1          # Tempo em segundos até expirar
TTL usuario:1000       # Tempo em segundos até expirar
PTTL produto:1         # Tempo em milissegundos até expirar
```

**Descrições**:

- `TTL chave`: Tempo restante em segundos (-1 = sem expiração, -2 = já expirou)
- `PTTL chave`: Mesmo que TTL mas retorna em milissegundos para maior precisão

## Buscar chaves por padrão

```redis
KEYS produto:*         # Todas as chaves que começam com "produto:"
KEYS usuario:*         # Todas as chaves que começam com "usuario:"
KEYS *:visitas         # Todas as chaves que terminam com ":visitas"
KEYS leaderboard:*     # Todas as chaves que começam com "leaderboard:"
```

**Descrição**: `KEYS padrão` usa wildcards (\*) para buscar chaves que correspondem ao padrão.
**⚠️ Atenção**: Comando lento em produção com muitas chaves, prefira `SCAN` em produção.

## Comandos úteis para análise

```redis
MEMORY USAGE produto:1 # Mostra quantos bytes a chave ocupa na memória
EXISTS produto:1       # Verifica se a chave existe (1=existe, 0=não existe)
STRLEN produto:1       # Retorna o tamanho da string em bytes
```

**Descrições**:

- `MEMORY USAGE chave`: Calcula o uso exato de memória da chave
- `EXISTS chave`: Verificação rápida de existência
- `STRLEN chave`: Tamanho da string (útil para strings/JSON)

## Limpar dados (se necessário)

```redis
DEL produto:1          # Remove uma chave específica
FLUSHDB               # Remove TODAS as chaves do banco atual
```

**Descrições**:

- `DEL chave`: Remove uma ou mais chaves específicas
- `FLUSHDB`: ⚠️ **PERIGOSO** - Apaga TUDO do banco atual (use com cuidado!)

## Exemplos de uso combinados

### Explorar todos os dados disponíveis

```redis
KEYS *                 # Lista todas as chaves para ver o que temos
TYPE produto:1         # Identifica que tipo de dado é
GET produto:1          # Se for string, pega o conteúdo
TYPE produto:1:visitas # Verifica o tipo do contador
GET produto:1:visitas  # Pega o valor atual do contador
TYPE usuario:1000      # Identifica como hash
HGETALL usuario:1000   # Mostra todos os campos do usuário
```

**Fluxo**: Primeiro identifica o que existe, depois verifica o tipo de cada chave, e finalmente consulta o conteúdo apropriado.

### Verificar TTL de todas as chaves

```redis
TTL produto:1          # Verifica expiração do cache de produto
TTL produto:2          # Verifica expiração do cache de produto 2
TTL usuario:1000       # Verifica expiração dos dados de usuário
```

**Objetivo**: Monitorar quando os dados vão expirar para entender o comportamento do cache.

### Estatísticas do Redis

```redis
INFO memory            # Mostra uso de memória do Redis
INFO stats             # Estatísticas gerais de operações
DBSIZE                # Conta total de chaves no banco
```

**Objetivo**: Monitoramento de performance e uso de recursos do Redis.

## 🎯 Dicas importantes

### Comandos seguros para produção

- `GET`, `HGET`, `SMEMBERS` - Sempre seguros
- `TTL`, `TYPE`, `EXISTS` - Operações de metadados, rápidas
- `DBSIZE` - Mais rápido que `KEYS *`

### Comandos que exigem cuidado

- `KEYS *` - Lento com muitas chaves, prefira `SCAN`
- `FLUSHDB` - Apaga tudo, use apenas em desenvolvimento
- `INFO` - Pode retornar muito texto

### Retornos especiais

- `TTL` retorna `-1` (sem expiração) ou `-2` (já expirou)
- `EXISTS` retorna `1` (existe) ou `0` (não existe)
- `SISMEMBER` retorna `1` (pertence ao set) ou `0` (não pertence)
- `TYPE` retorna: `string`, `hash`, `set`, `zset`, `list` ou `none`
