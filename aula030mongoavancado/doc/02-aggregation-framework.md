# Aggregation Framework no MongoDB

## Introdução

O **Aggregation Framework** é uma das funcionalidades mais poderosas do MongoDB, permitindo processar dados através de um pipeline de operações sequenciais. É ideal para análises complexas, transformações de dados e relatórios.

## Conceitos Fundamentais

### Pipeline de Agregação

O pipeline é uma sequência de estágios que processam documentos:
 
```javascript
db.collection.aggregate([
  { $stage1: { ... } },
  { $stage2: { ... } },
  { $stage3: { ... } }
])
```

### Fluxo de Dados

Cada estágio:

- Recebe documentos do estágio anterior
- Processa os documentos
- Passa os resultados para o próximo estágio

## Principais Estágios do Pipeline

### $match - Filtrar Documentos

Equivalente ao `find()`, filtra documentos baseado em critérios:

```javascript
// Filtrar pedidos de janeiro de 2024
db.pedidos.aggregate([
  {
    $match: {
      dataCreacao: {
        $gte: ISODate("2024-01-01"),
        $lt: ISODate("2024-02-01")
      }
    }
  }
])
```

### $project - Selecionar e Transformar Campos

Controla quais campos incluir/excluir e cria novos campos:

```javascript
db.pedidos.aggregate([
  {
    $project: {
      numeroPedido: 1,
      cliente: 1,
      total: 1,
      ano: { $year: "$dataCreacao" },
      mes: { $month: "$dataCreacao" },
      _id: 0
    }
  }
])
```

### $group - Agrupar e Agregar

Agrupa documentos e aplica operações de agregação:

```javascript
// Total de vendas por mês
db.pedidos.aggregate([
  {
    $group: {
      _id: {
        ano: { $year: "$dataCreacao" },
        mes: { $month: "$dataCreacao" }
      },
      totalVendas: { $sum: "$total" },
      quantidadePedidos: { $sum: 1 },
      ticketMedio: { $avg: "$total" }
    }
  }
])
```

### $sort - Ordenar Resultados

```javascript
db.pedidos.aggregate([
  { $group: { _id: "$cliente", total: { $sum: "$total" } } },
  { $sort: { total: -1 } } // Ordem decrescente
])
```

### $limit e $skip - Paginação

```javascript
db.pedidos.aggregate([
  { $sort: { dataCreacao: -1 } },
  { $skip: 10 },
  { $limit: 5 }
])
```

### $lookup - Join com Outras Coleções

```javascript
// Join pedidos com clientes
db.pedidos.aggregate([
  {
    $lookup: {
      from: "clientes",
      localField: "clienteId",
      foreignField: "_id",
      as: "dadosCliente"
    }
  }
])
```

### $unwind - Descompactar Arrays

```javascript
// Descompactar array de itens
db.pedidos.aggregate([
  { $unwind: "$itens" },
  {
    $group: {
      _id: "$itens.produto",
      quantidadeVendida: { $sum: "$itens.quantidade" }
    }
  }
])
```

## Operadores de Agregação

### Operadores Aritméticos

```javascript
{
  $project: {
    total: { $add: ["$subtotal", "$impostos"] },
    desconto: { $multiply: ["$total", 0.1] },
    diferenca: { $subtract: ["$precoOriginal", "$precoFinal"] }
  }
}
```

### Operadores de Comparação

```javascript
{
  $project: {
    categoria: {
      $cond: {
        if: { $gte: ["$total", 1000] },
        then: "Premium",
        else: "Standard"
      }
    }
  }
}
```

### Operadores de Data

```javascript
{
  $project: {
    ano: { $year: "$dataCreacao" },
    mes: { $month: "$dataCreacao" },
    diaSemana: { $dayOfWeek: "$dataCreacao" },
    trimestre: {
      $ceil: { $divide: [{ $month: "$dataCreacao" }, 3] }
    }
  }
}
```

### Operadores de String

```javascript
{
  $project: {
    nomeCompleto: { $concat: ["$nome", " ", "$sobrenome"] },
    iniciais: { $substr: ["$nome", 0, 1] },
    emailDominio: {
      $arrayElemAt: [{ $split: ["$email", "@"] }, 1]
    }
  }
}
```

### Operadores de Array

```javascript
{
  $project: {
    totalItens: { $size: "$itens" },
    primeiroItem: { $arrayElemAt: ["$itens", 0] },
    temDesconto: { $in: ["desconto", "$tags"] }
  }
}
```

## Exemplos Práticos Avançados

### Análise de Vendas Mensais

```javascript
db.pedidos.aggregate([
  {
    $match: {
      dataCreacao: { $gte: ISODate("2024-01-01") }
    }
  },
  {
    $group: {
      _id: {
        ano: { $year: "$dataCreacao" },
        mes: { $month: "$dataCreacao" }
      },
      receita: { $sum: "$total" },
      pedidos: { $sum: 1 },
      ticketMedio: { $avg: "$total" }
    }
  },
  {
    $project: {
      periodo: {
        $concat: [
          { $toString: "$_id.mes" },
          "/",
          { $toString: "$_id.ano" }
        ]
      },
      receita: { $round: ["$receita", 2] },
      pedidos: 1,
      ticketMedio: { $round: ["$ticketMedio", 2] }
    }
  },
  { $sort: { "_id.ano": 1, "_id.mes": 1 } }
])
```

### Top 10 Produtos Mais Vendidos

```javascript
db.pedidos.aggregate([
  { $unwind: "$itens" },
  {
    $group: {
      _id: "$itens.produto",
      quantidadeVendida: { $sum: "$itens.quantidade" },
      receitaTotal: { $sum: { $multiply: ["$itens.quantidade", "$itens.preco"] } }
    }
  },
  { $sort: { quantidadeVendida: -1 } },
  { $limit: 10 },
  {
    $project: {
      produto: "$_id",
      quantidadeVendida: 1,
      receitaTotal: { $round: ["$receitaTotal", 2] },
      _id: 0
    }
  }
])
```

### Análise de Comportamento do Cliente

```javascript
db.pedidos.aggregate([
  {
    $group: {
      _id: "$clienteId",
      totalPedidos: { $sum: 1 },
      valorTotal: { $sum: "$total" },
      primeiroPedido: { $min: "$dataCreacao" },
      ultimoPedido: { $max: "$dataCreacao" }
    }
  },
  {
    $project: {
      totalPedidos: 1,
      valorTotal: { $round: ["$valorTotal", 2] },
      ticketMedio: { $round: [{ $divide: ["$valorTotal", "$totalPedidos"] }, 2] },
      diasComoCliente: {
        $divide: [
          { $subtract: ["$ultimoPedido", "$primeiroPedido"] },
          1000 * 60 * 60 * 24
        ]
      },
      categoria: {
        $switch: {
          branches: [
            { case: { $gte: ["$valorTotal", 5000] }, then: "VIP" },
            { case: { $gte: ["$valorTotal", 1000] }, then: "Premium" },
            { case: { $gte: ["$totalPedidos", 5] }, then: "Frequente" }
          ],
          default: "Regular"
        }
      }
    }
  },
  { $sort: { valorTotal: -1 } }
])
```

## Otimização de Performance

### Posicionamento de $match

```javascript
// ✅ Correto - $match no início
db.pedidos.aggregate([
  { $match: { status: "concluido" } }, // Reduz dados desde o início
  { $group: { _id: "$cliente", total: { $sum: "$valor" } } }
])

// ❌ Incorreto - $match após processamento
db.pedidos.aggregate([
  { $group: { _id: "$cliente", total: { $sum: "$valor" } } },
  { $match: { total: { $gte: 1000 } } } // Processa todos os dados primeiro
])
```

### Uso de Índices

```javascript
// Criar índice composto para otimizar agregação
db.pedidos.createIndex({ "dataCreacao": 1, "status": 1 })

// Agregação que usa o índice
db.pedidos.aggregate([
  {
    $match: {
      dataCreacao: { $gte: ISODate("2024-01-01") },
      status: "concluido"
    }
  }
])
```

### Limit Early

```javascript
// Limitar resultados cedo no pipeline
db.pedidos.aggregate([
  { $sort: { dataCreacao: -1 } },
  { $limit: 100 }, // Limita antes de operações pesadas
  { $lookup: { ... } }
])
```

## Monitoramento e Debug

### Explain Plan

```javascript
db.pedidos.aggregate([...], { explain: true })
```

### Profile de Performance

```javascript
// Habilitar profiling
db.setProfilingLevel(2)

// Ver operações lentas
db.system.profile.find().sort({ ts: -1 }).limit(5)
```

## Exercícios Práticos

1. **Análise de E-commerce**: Criar relatório de vendas por categoria e região
2. **Log Analysis**: Analisar logs de acesso por IP e user agent
3. **Social Media**: Calcular engagement por post e usuário
4. **IoT Data**: Agregar dados de sensores por período e localização

## Limitações e Considerações

### Memory Limits

- Pipeline stages têm limite de 100MB de RAM
- Use `allowDiskUse: true` para datasets grandes

```javascript
db.collection.aggregate([...], { allowDiskUse: true })
```

### Document Size Limit

- Documentos resultantes limitados a 16MB
- Use `$project` para limitar campos retornados

## Referências

- [MongoDB Aggregation Pipeline](https://docs.mongodb.com/manual/core/aggregation-pipeline/)
- [Aggregation Pipeline Operators](https://docs.mongodb.com/manual/reference/operator/aggregation/)
- [Aggregation Pipeline Optimization](https://docs.mongodb.com/manual/core/aggregation-pipeline-optimization/)
