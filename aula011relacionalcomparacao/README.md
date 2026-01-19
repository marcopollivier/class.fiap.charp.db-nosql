# Aula 1.1: PostgreSQL vs SQL Server - O Que SQL Compartilha

> **Objetivo**: Mostrar que, apesar de arquiteturas diferentes, SGBDs relacionais maduros oferecem experiência SQL muito similar para operações básicas, preparando o contraste com NoSQL.

## 🎯 Por que Esta Comparação?

Antes de mergulhar no NoSQL, é importante entender que **dentro do mundo relacional**, as diferenças são principalmente de arquitetura interna, não de conceitos fundamentais.

### PostgreSQL vs SQL Server: Diferentes por Dentro, Similares por Fora

| Aspecto | PostgreSQL | SQL Server |
|---------|------------|------------|
| **Filosofia** | Extensibilidade, padrões abertos | Plataforma corporativa integrada |
| **Extensões** | PostGIS, pg_vector, tipos customizados | Recursos internos (Columnstore, In-Memory) |
| **JSON** | JSON/JSONB com índices GIN | Suporte via funções (melhorado recentemente) |
| **Linguagem** | PL/pgSQL + outras | T-SQL |
| **MVCC** | Implementação nativa | Row versioning + locking avançado |
| **Upsert** | `INSERT ... ON CONFLICT` | `MERGE` ou padrão `IF EXISTS` |
| **Licença** | PostgreSQL (liberal) | Proprietária (Developer gratuito) |

### O que Eles Compartilham (SQL Core)

- **DDL**: `CREATE TABLE`, `PRIMARY KEY`, `FOREIGN KEY`, `UNIQUE`
- **DML**: `INSERT`, `SELECT`, `UPDATE`, `DELETE`
- **JOINs**: `INNER`, `LEFT`, `RIGHT`, `FULL OUTER`
- **Agregações**: `COUNT`, `SUM`, `AVG`, `GROUP BY`, `HAVING`
- **Índices**: B-Tree padrão
- **Transações**: ACID garantido

## 🏗️ Estrutura do Laboratório

O laboratório inclui o **mesmo schema** implementado nos dois bancos:

```sql
-- Estrutura idêntica nos dois SGBDs
CREATE TABLE clientes (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE
);

CREATE TABLE pedidos (
    id SERIAL PRIMARY KEY,
    cliente_id INTEGER REFERENCES clientes(id),
    data_pedido TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    total DECIMAL(10,2)
);

CREATE TABLE itens_pedido (
    id SERIAL PRIMARY KEY,
    pedido_id INTEGER REFERENCES pedidos(id),
    produto VARCHAR(100),
    quantidade INTEGER,
    preco_unitario DECIMAL(10,2)
);
```

## 🚀 Como Executar

### 1. Subir os Bancos

```bash
docker compose up -d
```

Isso vai criar:

- **PostgreSQL**: `localhost:5432`
- **SQL Server**: `localhost:1433`

### 2. Conectar e Testar

**PostgreSQL:**

```bash
# Via psql
docker exec -it postgres-container psql -U postgres -d pedidos

# Via qualquer cliente SQL
Host: localhost:5432
Database: pedidos
User: postgres
Password: postgres
```

**SQL Server:**

```bash
# Via sqlcmd
docker exec -it sqlserver-container /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourPassword123"

# Via qualquer cliente SQL  
Host: localhost:1433
Database: pedidos
User: sa
Password: YourPassword123
```

## 🔍 Experimentos para Executar

### 1. Compare Sintaxes Similares

Execute estas queries nos dois bancos e veja como são praticamente idênticas:

```sql
-- Inserir dados (funciona igual nos dois)
INSERT INTO clientes (nome, email) VALUES ('João Silva', 'joao@email.com');
INSERT INTO clientes (nome, email) VALUES ('Maria Santos', 'maria@email.com');

-- Consultar com JOIN (sintaxe idêntica)
SELECT 
    c.nome,
    p.data_pedido,
    p.total
FROM clientes c
INNER JOIN pedidos p ON c.id = p.cliente_id
ORDER BY p.data_pedido DESC;

-- Agregações (funcionam igual)
SELECT 
    c.nome,
    COUNT(p.id) as total_pedidos,
    SUM(p.total) as valor_total
FROM clientes c
LEFT JOIN pedidos p ON c.id = p.cliente_id
GROUP BY c.id, c.nome;
```

### 2. Compare Diferenças Sutis

```sql
-- PostgreSQL: SERIAL
CREATE TABLE exemplo (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100)
);

-- SQL Server: IDENTITY
CREATE TABLE exemplo (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(100)
);
```

### 3. Teste Funcionalidades Específicas

**PostgreSQL - JSON nativo:**

```sql
-- PostgreSQL tem suporte nativo melhor para JSON
SELECT info->>'nome' as nome_json 
FROM clientes_json 
WHERE info @> '{"ativo": true}';
```

**SQL Server - Recursos corporativos:**

```sql
-- SQL Server tem recursos empresariais mais integrados
SELECT * FROM pedidos 
WITH (INDEX(IX_pedidos_data));
```

## 💡 O que Este Experimento Demonstra

### Similaridades (95% do Trabalho Diário)

- **CRUD básico**: Sintaxe quase idêntica
- **JOINs e relacionamentos**: Funcionam exatamente igual
- **Transações**: Comportamento ACID consistente
- **Índices simples**: B-Tree funciona igual
- **Constraints**: `FOREIGN KEY`, `UNIQUE`, `CHECK` similares

### Diferenças (5% - Casos Específicos)

- **Tipos específicos**: `SERIAL` vs `IDENTITY`
- **Funções avançadas**: JSON, full-text, geoespacial
- **Sintaxe de upsert**: `ON CONFLICT` vs `MERGE`
- **Extensões**: PostgreSQL mais flexível
- **Ferramentas**: Ecossistema diferente

## 🎯 Principais Insights

1. **SQL é SQL**: As operações fundamentais são praticamente idênticas
2. **Diferenças são na periferia**: Tipos específicos, extensões, funções avançadas
3. **Migração é viável**: O core da aplicação geralmente funciona sem grandes mudanças
4. **Preparação para NoSQL**: Esta similaridade torna o contraste com NoSQL mais evidente

## 🔗 Próximo Passo

Agora que você viu como bancos relacionais são similares no core, vamos ver como **MongoDB** quebra completamente esses paradigmas na próxima aula.

A diferença entre PostgreSQL → SQL Server é evolução.  
A diferença entre SQL → NoSQL é revolução.
init-postgres/
  01-schema.sql
  02-crud.sql
init-sqlserver/
  01-schema.sql
  02-crud.sql
Makefile

```

## Modelo

Tabelas: `clientes` e `pedidos` (1 cliente possui vários pedidos). Regra de negócio simples: valor >= 0.

## Subindo Ambiente

Pré-requisito: Docker.

```bash
cd aula011relacionalcomparacao
make up
make ps
```

Aguarde healthchecks ficarem `healthy`.

## Aplicando CRUD de exemplo

PostgreSQL:

```bash
make seed-postgres
make psql
```

Dentro do `psql` você pode repetir o SELECT final:

```sql
SELECT c.id, c.nome, COUNT(p.id) AS total_pedidos
FROM clientes c
LEFT JOIN pedidos p ON p.cliente_id = c.id
GROUP BY c.id, c.nome
ORDER BY c.id;
```

SQL Server:

```bash
make seed-sqlserver
make sqlcmd
```

No `sqlcmd` rode:

```sql
SELECT c.id, c.nome, COUNT(p.id) AS total_pedidos
FROM clientes c
LEFT JOIN pedidos p ON p.cliente_id = c.id
GROUP BY c.id, c.nome
ORDER BY c.id;
GO
```

## Limpeza

```bash
make down
```

Para remover volumes (reset total):

```bash
make clean
```

## Discussão

- Diferenças de sintaxe pontuais: tipos (`SERIAL` vs `IDENTITY`), timestamps (`TIMESTAMPTZ` vs `DATETIME2`), upsert (`ON CONFLICT` vs `MERGE`).
- Similaridade operacional: criação de chave estrangeira, integridade referencial, cascata, índices.
- Prepara contraste para NoSQL: quando estrutura flexível, ausência de JOINS e modelagem orientada a documento ou chave-valor faz sentido.

## Próximos Passos

- Introduzir consultas agregadas mais complexas.
- Mostrar funções específicas (JSONB vs JSON).
- Migrar esse modelo para variante NoSQL em aulas seguintes.

## Segurança (educacional)

Senhas e usuários simplificados. Em produção: usar variáveis de ambiente seguras, usuários com menos privilégios, TLS, políticas de backup.

---

Aproveite a exploração! Compare tempo de resposta, consumo de memória, e recursos adicionais conforme necessário.
