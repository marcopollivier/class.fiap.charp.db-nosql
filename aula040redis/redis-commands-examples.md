# Exemplos de Comandos Redis para Demonstração

## Como usar este arquivo

1. Execute `make redis-cli` para conectar ao Redis
2. Cole os comandos abaixo um por vez
3. Observe os resultados

## 1. Strings - Operações Básicas

```redis
# Configurar valores
SET produto:1 "Smartphone Galaxy"
SET produto:1:preco 899.99
SET produto:1:estoque 50

# Recuperar valores
GET produto:1
GET produto:1:preco

# Incrementar valores numéricos
INCR produto:1:visitas
INCR produto:1:visitas
INCR produto:1:visitas

# Decrementar estoque
DECR produto:1:estoque
DECRBY produto:1:estoque 5

# TTL (Time To Live)
SET sessao:abc123 "dados_usuario" EX 300  # Expira em 5 minutos
TTL sessao:abc123

# Múltiplas operações
MSET produto:2 "iPhone" produto:2:preco 999.99 produto:2:estoque 30
MGET produto:1 produto:2 produto:1:preco produto:2:preco
```

## 2. Hashes - Objetos Estruturados

```redis
# Criar usuário como hash
HMSET usuario:1000 nome "João Silva" email "joao@email.com" idade 30 ativo true

# Ler campos específicos
HGET usuario:1000 nome
HGET usuario:1000 email

# Ler todos os campos
HGETALL usuario:1000

# Incrementar campo numérico
HINCRBY usuario:1000 idade 1

# Verificar se campo existe
HEXISTS usuario:1000 telefone

# Adicionar novo campo
HSET usuario:1000 telefone "11999999999"

# Obter todas as chaves
HKEYS usuario:1000

# Obter todos os valores
HVALS usuario:1000
```

## 3. Lists - Filas e Listas

```redis
# Adicionar itens (pilha - LIFO)
LPUSH notificacoes:user:1000 "Bem-vindo!"
LPUSH notificacoes:user:1000 "Você tem uma nova mensagem"
LPUSH notificacoes:user:1000 "Sua compra foi aprovada"

# Ver lista completa
LRANGE notificacoes:user:1000 0 -1

# Implementar fila (FIFO)
RPUSH fila:pedidos "pedido:1001"
RPUSH fila:pedidos "pedido:1002"
RPUSH fila:pedidos "pedido:1003"

# Processar fila
LPOP fila:pedidos  # Remove e retorna o primeiro
LPOP fila:pedidos

# Tamanho da lista
LLEN fila:pedidos

# Buscar por índice
LINDEX notificacoes:user:1000 0  # Primeira notificação
LINDEX notificacoes:user:1000 -1 # Última notificação
```

## 4. Sets - Coleções Únicas

```redis
# Adicionar tags a um produto
SADD produto:1:tags "smartphone"
SADD produto:1:tags "android"
SADD produto:1:tags "camera"
SADD produto:1:tags "bluetooth"

# Ver todas as tags
SMEMBERS produto:1:tags

# Verificar se tag existe
SISMEMBER produto:1:tags "android"
SISMEMBER produto:1:tags "ios"

# Adicionar tags a outro produto
SADD produto:2:tags "smartphone"
SADD produto:2:tags "ios"
SADD produto:2:tags "camera"

# Operações entre conjuntos
SINTER produto:1:tags produto:2:tags  # Interseção (tags comuns)
SUNION produto:1:tags produto:2:tags  # União (todas as tags)
SDIFF produto:1:tags produto:2:tags   # Diferença (só em produto:1)

# Contar elementos
SCARD produto:1:tags
```

## 5. Sorted Sets - Rankings e Leaderboards

```redis
# Adicionar pontuações
ZADD leaderboard:game 1500 "player1"
ZADD leaderboard:game 2300 "player2"
ZADD leaderboard:game 1800 "player3"
ZADD leaderboard:game 2100 "player4"
ZADD leaderboard:game 1200 "player5"

# Ver ranking (maior pontuação primeiro)
ZREVRANGE leaderboard:game 0 -1 WITHSCORES

# Top 3
ZREVRANGE leaderboard:game 0 2 WITHSCORES

# Posição de um jogador específico
ZREVRANK leaderboard:game "player2"  # 0-indexed

# Incrementar pontuação
ZINCRBY leaderboard:game 100 "player1"

# Pontuação de um jogador
ZSCORE leaderboard:game "player1"

# Jogadores por faixa de pontuação
ZRANGEBYSCORE leaderboard:game 1500 2000 WITHSCORES

# Contar jogadores
ZCARD leaderboard:game
```

## 6. Expiração e TTL

```redis
# Definir TTL ao criar
SETEX cache:dados 60 "valor temporário"  # Expira em 60 segundos

# Definir TTL em chave existente
EXPIRE produto:1:tags 3600  # Expira em 1 hora

# Ver tempo restante
TTL produto:1:tags

# Remover TTL (tornar permanente)
PERSIST produto:1:tags

# TTL em milissegundos
PTTL cache:dados
```

## 7. Operações Avançadas

```redis
# Transações
MULTI
SET conta:1 100
SET conta:2 200
DECRBY conta:1 50
INCRBY conta:2 50
EXEC

# Pipeline simulado (executar vários comandos)
SET key1 "value1"
SET key2 "value2"
SET key3 "value3"
MGET key1 key2 key3

# Buscar chaves por padrão
KEYS produto:*
KEYS *:tags

# Informações do servidor
INFO memory
INFO stats
INFO server

# Limpar database atual (CUIDADO!)
# FLUSHDB

# Ver todas as chaves
KEYS *

# Contar chaves
DBSIZE
```

## 8. Rate Limiting com TTL

```redis
# Implementar rate limiting
SET rate:user:1000:2024-11-09-14-30 1 EX 60  # 1 request na janela de 1 minuto
INCR rate:user:1000:2024-11-09-14-30
INCR rate:user:1000:2024-11-09-14-30
GET rate:user:1000:2024-11-09-14-30
TTL rate:user:1000:2024-11-09-14-30
```

## 9. Pub/Sub (Demonstração Básica)

```redis
# Em uma sessão, inscrever-se em canal
SUBSCRIBE noticias

# Em outra sessão (abra outro terminal: make redis-cli)
PUBLISH noticias "Nova funcionalidade disponível!"

# Volta para primeira sessão para ver a mensagem
# Ctrl+C para sair do modo subscribe
```

## 10. Monitoring e Debug

```redis
# Monitor todos os comandos (CUIDADO: muito verboso)
# MONITOR

# Estatísticas
INFO stats

# Comandos mais lentos
SLOWLOG GET 10

# Clientes conectados
CLIENT LIST

# Uso de memória
MEMORY USAGE produto:1:tags
INFO memory
```

## 11. Cleanup (Limpeza)

```redis
# Remover chaves específicas
DEL produto:1
DEL produto:1:preco
DEL produto:1:estoque

# Remover por padrão (cuidado!)
# EVAL "return redis.call('del', unpack(redis.call('keys', ARGV[1])))" 0 produto:*

# Ver o que sobrou
KEYS *
DBSIZE
```

## Dicas de Uso

1. **Use TTL apropriado**: Evite chaves que nunca expiram
2. **Nomeação consistente**: Use padrões como `entidade:id:campo`
3. **Tipos apropriados**: Escolha o tipo de dados correto para cada caso
4. **Monitoramento**: Use INFO e SLOWLOG para monitorar performance
5. **Transações**: Use MULTI/EXEC para operações atômicas

## Comandos de Administração

```redis
# Backup manual
BGSAVE

# Ver configurações
CONFIG GET save
CONFIG GET maxmemory

# Informações de persistência
LASTSAVE
INFO persistence
```
