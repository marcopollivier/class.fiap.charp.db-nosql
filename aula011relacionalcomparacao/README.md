# Aula 011 - Comparação Relacional: PostgreSQL vs SQL Server

## Objetivo

Demonstrar que, apesar de arquiteturas e filosofias diferentes, dois SGBDs relacionais maduros compartilham uma experiência SQL muito similar para operações de CRUD e modelagem simples (1:N), preparando terreno para discutir contrastes com bancos NoSQL.

## Por que PostgreSQL e SQL Server?

| Aspecto | PostgreSQL | SQL Server |
|---------|------------|------------|
| Filosofia | Extensibilidade, padrões abertos, comunidade | Plataforma corporativa integrada (ecosistema MS) |
| Extensões | PostGIS, pg_vector, tipos customizados | Recursos avançados internos (Columnstore, In-Memory OLTP) |
| Tipos JSON | JSON / JSONB com índices GIN | JSON via funções (sem tipo nativo até versões recentes) |
| Linguagem Procedural | PL/pgSQL + outras | T-SQL |
| MVCC | Implementado nativamente | Implementado com versão row e locking sofisticado |
| Upsert | `INSERT ... ON CONFLICT` | `MERGE` ou padrão `IF EXISTS` |
| Licença | PostgreSQL (liberal) | Proprietária (Developer grátis) |
| Ferramentas CLI | `psql` | `sqlcmd` |

Apesar das diferenças, você encontrará:

- DDL: CREATE TABLE, constraints PRIMARY KEY, UNIQUE, FOREIGN KEY.
- DML: INSERT, SELECT, UPDATE, DELETE.
- JOINS e agregações.
- Índices B-Tree padrão.
- Transações ACID.

## Estrutura

```text
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
