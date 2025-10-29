-- Schema inicial SQL Server
CREATE TABLE clientes (
  id INT IDENTITY(1,1) PRIMARY KEY,
  nome NVARCHAR(150) NOT NULL,
  email NVARCHAR(200) NOT NULL UNIQUE,
  criado_em DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE pedidos (
  id INT IDENTITY(1,1) PRIMARY KEY,
  cliente_id INT NOT NULL,
  descricao NVARCHAR(250) NOT NULL,
  valor DECIMAL(10,2) NOT NULL,
  criado_em DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  
  CONSTRAINT FK_pedidos_clientes FOREIGN KEY (cliente_id) 
  REFERENCES clientes(id) ON DELETE CASCADE,
  
  CONSTRAINT CK_pedidos_valor CHECK (valor >= 0)
);

CREATE INDEX idx_pedidos_cliente_id ON pedidos(cliente_id);
