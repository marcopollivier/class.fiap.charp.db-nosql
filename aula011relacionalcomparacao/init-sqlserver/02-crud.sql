-- CRUD básico SQL Server
-- Insert cliente
INSERT INTO clientes (nome, email) VALUES (N'Alice', N'alice@example.com');
INSERT INTO clientes (nome, email) VALUES (N'Bob', N'bob@example.com');

-- Upsert cliente (MERGE simulando ON CONFLICT)
MERGE clientes AS target
USING (SELECT N'Alice Nova' AS nome, N'alice@example.com' AS email) AS source
ON (target.email = source.email)
WHEN MATCHED THEN UPDATE SET nome = source.nome
WHEN NOT MATCHED THEN INSERT (nome, email) VALUES (source.nome, source.email);

-- Insert pedido
INSERT INTO pedidos (cliente_id, descricao, valor) VALUES (1, N'Pedido A1', 100.00);
INSERT INTO pedidos (cliente_id, descricao, valor) VALUES (1, N'Pedido A2', 150.00);
INSERT INTO pedidos (cliente_id, descricao, valor) VALUES (2, N'Pedido B1', 200.00);

-- Select join
SELECT c.id, c.nome, p.descricao, p.valor
FROM clientes c
JOIN pedidos p ON p.cliente_id = c.id
ORDER BY c.id, p.id;

-- Update pedido
UPDATE pedidos SET valor = valor + 10 WHERE id = 1;

-- Delete pedido
DELETE FROM pedidos WHERE id = 2;

-- Delete cliente (cascade apaga pedidos)
DELETE FROM clientes WHERE email = N'bob@example.com';

-- Mostrar estado final
SELECT c.id, c.nome, COUNT(p.id) AS total_pedidos
FROM clientes c
LEFT JOIN pedidos p ON p.cliente_id = c.id
GROUP BY c.id, c.nome
ORDER BY c.id;
