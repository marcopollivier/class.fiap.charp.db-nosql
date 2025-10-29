-- CRUD básico PostgreSQL
-- Insert cliente
INSERT INTO clientes (nome, email) VALUES ('Alice', 'alice@example.com');
INSERT INTO clientes (nome, email) VALUES ('Bob', 'bob@example.com');

-- Upsert cliente (email como chave única)
INSERT INTO clientes (nome, email) VALUES ('Alice Nova', 'alice@example.com')
ON CONFLICT (email) DO UPDATE SET nome = EXCLUDED.nome;

-- Insert pedido
INSERT INTO pedidos (cliente_id, descricao, valor) VALUES (1, 'Pedido A1', 100.00);
INSERT INTO pedidos (cliente_id, descricao, valor) VALUES (1, 'Pedido A2', 150.00);
INSERT INTO pedidos (cliente_id, descricao, valor) VALUES (2, 'Pedido B1', 200.00);

-- Select join
SELECT c.id, c.nome, p.descricao, p.valor
FROM clientes c
JOIN pedidos p ON p.cliente_id = c.id
ORDER BY c.id, p.id;

-- Update pedido
UPDATE pedidos SET valor = valor + 10 WHERE id = 1;

-- Delete pedido
DELETE FROM pedidos WHERE id = 2;

-- Delete cliente (cascata apaga pedidos)
DELETE FROM clientes WHERE email = 'bob@example.com';

-- Mostrar estado final
SELECT c.id, c.nome, COUNT(p.id) AS total_pedidos
FROM clientes c
LEFT JOIN pedidos p ON p.cliente_id = c.id
GROUP BY c.id, c.nome
ORDER BY c.id;
