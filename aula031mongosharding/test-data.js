// MongoDB Sharding - Dados de teste para distribuição por inicial do nome

// Use este script para testar a distribuição de sharding
use pedidos

// Dados para Shard 1 (A-H)
db.clientes.insertMany([
    {nome: "Ana Silva", email: "ana@teste.com", primeiraLetra: "A"},
    {nome: "Bruno Costa", email: "bruno@teste.com", primeiraLetra: "B"},
    {nome: "Carlos Oliveira", email: "carlos@teste.com", primeiraLetra: "C"},
    {nome: "Diana Santos", email: "diana@teste.com", primeiraLetra: "D"},
    {nome: "Eduardo Lima", email: "eduardo@teste.com", primeiraLetra: "E"},
    {nome: "Fernanda Souza", email: "fernanda@teste.com", primeiraLetra: "F"},
    {nome: "Gabriel Alves", email: "gabriel@teste.com", primeiraLetra: "G"},
    {nome: "Helena Rodrigues", email: "helena@teste.com", primeiraLetra: "H"}
])

// Dados para Shard 2 (I-P)
db.clientes.insertMany([
    {nome: "Igor Ferreira", email: "igor@teste.com", primeiraLetra: "I"},
    {nome: "Julia Machado", email: "julia@teste.com", primeiraLetra: "J"},
    {nome: "Kelly Mendes", email: "kelly@teste.com", primeiraLetra: "K"},
    {nome: "Lucas Barbosa", email: "lucas@teste.com", primeiraLetra: "L"},
    {nome: "Maria José", email: "maria@teste.com", primeiraLetra: "M"},
    {nome: "Nicolas Pereira", email: "nicolas@teste.com", primeiraLetra: "N"},
    {nome: "Otavio Gomes", email: "otavio@teste.com", primeiraLetra: "O"},
    {nome: "Patricia Cardoso", email: "patricia@teste.com", primeiraLetra: "P"}
])

// Dados para Shard 3 (Q-Z)
db.clientes.insertMany([
    {nome: "Quirino Ramos", email: "quirino@teste.com", primeiraLetra: "Q"},
    {nome: "Roberto Silva", email: "roberto@teste.com", primeiraLetra: "R"},
    {nome: "Sofia Oliveira", email: "sofia@teste.com", primeiraLetra: "S"},
    {nome: "Thiago Santos", email: "thiago@teste.com", primeiraLetra: "T"},
    {nome: "Ursula Lima", email: "ursula@teste.com", primeiraLetra: "U"},
    {nome: "Vitor Souza", email: "vitor@teste.com", primeiraLetra: "V"},
    {nome: "Wagner Alves", email: "wagner@teste.com", primeiraLetra: "W"},
    {nome: "Ximena Rodrigues", email: "ximena@teste.com", primeiraLetra: "X"},
    {nome: "Yara Ferreira", email: "yara@teste.com", primeiraLetra: "Y"},
    {nome: "Zeca Pagodinho", email: "zeca@teste.com", primeiraLetra: "Z"}
])

// Verificar distribuição
db.clientes.getShardDistribution()

// Contar por shard
print("=== DISTRIBUIÇÃO POR SHARD ===")
print("Shard 1 (A-H): " + db.clientes.find({primeiraLetra: {$gte: "A", $lte: "H"}}).count())
print("Shard 2 (I-P): " + db.clientes.find({primeiraLetra: {$gte: "I", $lte: "P"}}).count())
print("Shard 3 (Q-Z): " + db.clientes.find({primeiraLetra: {$gte: "Q", $lte: "Z"}}).count())
print("Total: " + db.clientes.find().count())

// Testar consultas específicas
print("=== CONSULTAS DE TESTE ===")
print("Clientes com inicial 'A': " + db.clientes.find({primeiraLetra: "A"}).count())
print("Clientes com inicial 'M': " + db.clientes.find({primeiraLetra: "M"}).count())
print("Clientes com inicial 'Z': " + db.clientes.find({primeiraLetra: "Z"}).count())