-- Schema inicial PostgreSQL
CREATE TABLE clientes (
  id SERIAL PRIMARY KEY,
  nome VARCHAR(150) NOT NULL,
  email VARCHAR(200) NOT NULL UNIQUE,
  criado_em TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE pedidos (
  id SERIAL PRIMARY KEY,
  
  cliente_id INT NOT NULL 
  REFERENCES clientes(id) ON DELETE CASCADE,
  
  descricao VARCHAR(250) NOT NULL,
  valor NUMERIC(10,2) NOT NULL CHECK (valor >= 0),
  criado_em TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_pedidos_cliente_id ON pedidos(cliente_id);
