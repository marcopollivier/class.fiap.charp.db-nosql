# Aula 2: MongoDB - Estrutura Básica

> **Objetivo**: Entender os conceitos fundamentais do MongoDB através de um setup automatizado que "sempre funciona" - ideal para aprendizado e experimentação.

## 🎯 Por que Este Setup?

"Como MongoDB organiza dados?" e "Como fazer um ambiente que sempre funciona para estudar?"

Este laboratório resolve ambas as questões: setup automatizado + dados pré-carregados para você focar no que importa.

## � Conceitos MongoDB que Você Vai Ver

### Documentos vs Registros

```javascript
// SQL: 3 tabelas relacionadas
Clientes: { id: 1, nome: "João Silva" }
Pedidos:  { id: 101, cliente_id: 1, data: "2024-01-01" }
Itens:    { pedido_id: 101, produto: "Notebook", preco: 4500 }

// MongoDB: 1 documento completo
{
  _id: ObjectId("..."),
  cliente: "João Silva",
  data: ISODate("2024-01-01"),
  itens: [
    { produto: "Notebook", quantidade: 1, preco: 4500 },
    { produto: "Mouse", quantidade: 1, preco: 150 }
  ],
  total: 4650
}
```

### Collections vs Tabelas

- **Sem schema fixo**: Documentos na mesma collection podem ter estruturas diferentes
- **Aninhamento**: Dados relacionados ficam juntos (embedded)
- **Arrays**: Lista de itens dentro do documento
- **Flexibilidade**: Adicionar campos sem `ALTER TABLE`

## 📋 Dados Pré-carregados

O setup cria automaticamente:

### Collection: `clientes`

```javascript
[
  { nome: "João Silva", email: "joao@exemplo.com" },
  { nome: "Maria Santos", email: "maria@exemplo.com" }
]
```

### Collection: `pedidos`

```javascript
[
  {
    cliente: "João Silva",
    data: new Date(),
    itens: [
      { produto: "Notebook", quantidade: 1, preco: 4500 },
      { produto: "Mouse", quantidade: 1, preco: 150 }
    ],
    total: 4650
  }
]
```

## 🚀 Como Usar

### 1. Iniciar o Ambiente

```bash
# Na pasta da aula
make up
```

**O que acontece:**

- Limpa dados antigos automaticamente
- Inicia MongoDB na porta 27017
- Carrega dados iniciais
- Pronto para uso!

### 2. Conectar e Explorar

```bash
# Conectar no MongoDB shell
docker exec -it aula02-mongodb mongosh -u admin -p password123 --authenticationDatabase admin pedidos

# Dentro do mongosh:
db.clientes.find()                    # Ver todos os clientes
db.pedidos.find().pretty()            # Ver pedidos formatados
db.pedidos.findOne()                  # Ver um pedido completo
```

### 3. Experimentar Consultas

```javascript
// Buscar pedidos de um cliente específico
db.pedidos.find({ cliente: "João Silva" })

// Buscar pedidos com valor total acima de 1000
db.pedidos.find({ total: { $gt: 1000 } })

// Buscar pedidos que contém notebook
db.pedidos.find({ "itens.produto": "Notebook" })

// Contar documentos
db.pedidos.countDocuments()
```

### 4. Limpar o Ambiente

```bash
make down
```

## � Experimentos Sugeridos

### 1. Compare com SQL

Execute estas consultas e pense como seria em SQL:

```javascript
// MongoDB: Busca direta sem JOIN
db.pedidos.find({ cliente: "João Silva" })

// SQL equivalente precisaria de JOIN:
// SELECT p.*, c.nome FROM pedidos p 
// JOIN clientes c ON p.cliente_id = c.id 
// WHERE c.nome = 'João Silva'
```

### 2. Teste a Flexibilidade

```javascript
// Adicionar novo pedido com estrutura diferente
db.pedidos.insertOne({
  cliente: "Maria Santos",
  data: new Date(),
  itens: [
    { produto: "Livro", quantidade: 3, preco: 50, categoria: "educacao" }
  ],
  total: 150,
  desconto: 10,  // Campo novo!
  observacoes: "Entrega urgente"  // Outro campo novo!
})

// Funciona! Sem ALTER TABLE necessário
```

### 3. Explore Queries com Arrays

```javascript
// Buscar pedidos com mais de 1 item
db.pedidos.find({ "itens.1": { $exists: true } })

// Buscar por produto específico
db.pedidos.find({ "itens.produto": "Mouse" })

// Somar total de itens
db.pedidos.aggregate([
  { $unwind: "$itens" },
  { $group: { _id: null, totalItens: { $sum: "$itens.quantidade" } } }
])
```

## 📁 Estrutura do Projeto

```text
aula020mongodbestruturabasica/
├── Makefile                    # Automação (make up/down)
├── docker-compose.yml          # Configuração do MongoDB
├── init-mongo.js              # Script com dados iniciais
├── README.md                  # Esta documentação
└── data/
    └── db/                    # Dados persistentes (ignorado pelo git)
```

## � Por que Este Approach?

### ✅ Sempre Funciona

- **Dados limpos**: Sempre inicia com estado conhecido
- **Automação**: Um comando e está pronto
- **Reproduzível**: Funciona igual para todos

### ✅ Ideal para Aprendizado

- **Dados de exemplo**: Já tem dados para testar
- **Foco no MongoDB**: Não perde tempo com setup
- **Experimentos seguros**: `make down` limpa tudo

### ✅ Realista

- **Dados persistentes**: Armazenados localmente
- **Versionamento inteligente**: Estrutura no git, dados não
- **Docker profissional**: Setup parecido com produção

## 🎯 Principais Insights

Após executar os experimentos:

1. **Flexibilidade**: Documentos podem ter estruturas diferentes
2. **Simplicidade**: Consultas diretas sem JOINs complexos
3. **Aninhamento**: Dados relacionados ficam juntos
4. **Performance**: Menos consultas para dados relacionados

## 🔗 Próximos Passos

- **Aula 3**: MongoDB avançado (agregações, índices, modelagem)
- **Compare**: Como esta flexibilidade impacta performance?
- **Pense**: Quando documentos aninhados são melhores que tabelas relacionadas?

## 🛠️ Comandos Disponíveis

```bash
make up       # Inicia MongoDB com dados frescos
make down     # Para e limpa tudo
make logs     # Ver logs do MongoDB
make shell    # Conectar no mongosh
```
