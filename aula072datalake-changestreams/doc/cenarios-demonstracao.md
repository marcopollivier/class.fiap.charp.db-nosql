# Cenários de Demonstração - Data Lake com Change Streams

## Cenário 1: E-commerce em Tempo Real

### Objetivo
Demonstrar como um e-commerce pode capturar e processar eventos de vendas em tempo real usando Change Streams para alimentar dashboards analíticos.

### Simulação

```javascript
// 1. Cadastro de novo cliente
db.clientes.insertOne({
  nome: "Carlos Oliveira",
  email: "carlos.oliveira@email.com"
});

// 2. Cliente faz um pedido
const cliente = db.clientes.findOne({email: "carlos.oliveira@email.com"});

db.pedidos.insertOne({
  clienteId: cliente._id,
  dataPedido: new Date(),
  itens: [
    {
      itemId: ObjectId("6564f1a2b8c9d4e5f6789ac0"),
      nome: "Notebook Dell",
      preco: 2500.00,
      quantidade: 1
    },
    {
      itemId: ObjectId("6564f1a2b8c9d4e5f6789ac1"),
      nome: "Mouse Logitech",
      preco: 75.00,
      quantidade: 2
    }
  ],
  total: 2650.00
});

// 3. Cliente atualiza o pedido (adiciona item)
const pedido = db.pedidos.findOne({clienteId: cliente._id});

db.pedidos.updateOne(
  {_id: pedido._id},
  {
    $push: {
      itens: {
        itemId: ObjectId("6564f1a2b8c9d4e5f6789ac2"),
        nome: "Teclado Mecânico", 
        preco: 350.00,
        quantidade: 1
      }
    },
    $set: { total: 3000.00 }
  }
);

// 4. Verificar eventos gerados
print("=== EVENTOS DO CLIENTE ===");
db.datalake.find({
  "pedido.cliente.email": "carlos.oliveira@email.com"
}).sort({timestamp: 1}).forEach(evento => {
  print(`[${evento.timestamp.toISOString()}] ${evento.evento} - Total: R$ ${evento.pedido?.total || 'N/A'}`);
});
```

**Resultado Esperado**: 
- 3 eventos no Data Lake: cliente_insert, pedido_insert, pedido_update
- Dados denormalizados com informações completas do cliente em cada evento de pedido

## Cenário 2: Gestão de Estoque

### Objetivo
Monitorar mudanças de estoque e gerar alertas automáticos quando itens ficam com baixa quantidade.

### Simulação

```javascript
// 1. Verificar estoque atual
print("=== ESTOQUE ATUAL ===");
db.itens.find({}, {nome: 1, quantidade: 1}).forEach(item => {
  print(`${item.nome}: ${item.quantidade} unidades`);
});

// 2. Simular vendas que reduzem estoque
db.itens.updateOne(
  {nome: "Mouse Logitech"},
  {$inc: {quantidade: -45}} // Reduz para 5 unidades
);

db.itens.updateOne(
  {nome: "Teclado Mecânico"},
  {$inc: {quantidade: -20}} // Reduz para 5 unidades
);

// 3. Item com estoque crítico
db.itens.updateOne(
  {nome: "Monitor 24pol"},
  {$inc: {quantidade: -13}} // Reduz para 2 unidades
);

// 4. Consulta analítica: Itens com estoque baixo
print("=== ANÁLISE DE ESTOQUE (Baseada no Data Lake) ===");
db.datalake.aggregate([
  { $match: { evento: "item_update" } },
  { $sort: { timestamp: -1 } },
  { $group: {
    _id: "$documentId",
    ultimoEvento: { $first: "$$ROOT" }
  }},
  { $lookup: {
    from: "itens",
    localField: "_id",
    foreignField: "_id",
    as: "item"
  }},
  { $unwind: "$item" },
  { $match: { "item.quantidade": { $lt: 10 } } },
  { $project: {
    nome: "$item.nome",
    quantidade: "$item.quantidade",
    ultimaAlteracao: "$ultimoEvento.timestamp"
  }}
]).forEach(result => {
  print(`⚠️  ${result.nome}: ${result.quantidade} unidades (Última alteração: ${result.ultimaAlteracao})`);
});
```

## Cenário 3: Auditoria de Mudanças

### Objetivo
Demonstrar como usar o Data Lake para auditoria completa de mudanças em dados críticos.

### Simulação

```javascript
// 1. Mudança de preço de produto
const itemOriginal = db.itens.findOne({nome: "Notebook Dell"});
print(`Preço original: R$ ${itemOriginal.preco}`);

db.itens.updateOne(
  {nome: "Notebook Dell"},
  {$set: {preco: 2300.00}} // Redução de preço
);

// 2. Correção de email de cliente
db.clientes.updateOne(
  {nome: "João Silva"},
  {$set: {email: "joao.silva.novo@email.com"}}
);

// 3. Cancelamento de pedido (exclusão)
const pedidoCancelado = db.pedidos.findOne({total: {$lt: 500}});
if (pedidoCancelado) {
  print(`Cancelando pedido: ${pedidoCancelado._id}`);
  db.pedidos.deleteOne({_id: pedidoCancelado._id});
}

// 4. Relatório de auditoria
print("\n=== RELATÓRIO DE AUDITORIA (Últimas alterações) ===");
db.datalake.find().sort({timestamp: -1}).limit(10).forEach(evento => {
  const time = evento.timestamp.toISOString().substring(11, 19);
  const operacao = evento.operacao.toUpperCase();
  
  print(`[${time}] ${operacao} em ${evento.colecao}`);
  print(`   Evento: ${evento.evento}`);
  print(`   Documento: ${evento.documentId}`);
  
  if (evento.pedido) {
    print(`   Cliente: ${evento.pedido.cliente.nome} - Total: R$ ${evento.pedido.total}`);
  }
  print("---");
});
```

## Cenário 4: Analytics em Tempo Real

### Objetivo
Gerar métricas de negócio em tempo real usando os dados do Data Lake.

### Simulação

```javascript
// 1. Dashboard de vendas do dia
print("=== DASHBOARD DE VENDAS - HOJE ===");

const hoje = new Date();
hoje.setHours(0,0,0,0);

// Total de vendas do dia
const vendasHoje = db.datalake.aggregate([
  {
    $match: {
      evento: "pedido_insert",
      timestamp: { $gte: hoje }
    }
  },
  {
    $group: {
      _id: null,
      totalVendas: { $sum: "$pedido.total" },
      quantidadePedidos: { $sum: 1 },
      ticketMedio: { $avg: "$pedido.total" }
    }
  }
]).toArray()[0];

if (vendasHoje) {
  print(`💰 Total de vendas: R$ ${vendasHoje.totalVendas.toFixed(2)}`);
  print(`📦 Pedidos realizados: ${vendasHoje.quantidadePedidos}`);
  print(`🎯 Ticket médio: R$ ${vendasHoje.ticketMedio.toFixed(2)}`);
} else {
  print("📊 Nenhuma venda registrada hoje");
}

// Top clientes do dia
print("\n=== TOP CLIENTES DO DIA ===");
db.datalake.aggregate([
  {
    $match: {
      evento: "pedido_insert",
      timestamp: { $gte: hoje }
    }
  },
  {
    $group: {
      _id: "$pedido.cliente.nome",
      totalGasto: { $sum: "$pedido.total" },
      quantidadePedidos: { $sum: 1 }
    }
  },
  { $sort: { totalGasto: -1 } },
  { $limit: 3 }
]).forEach((cliente, index) => {
  const posicao = index + 1;
  print(`${posicao}º ${cliente._id} - R$ ${cliente.totalGasto.toFixed(2)} (${cliente.quantidadePedidos} pedidos)`);
});

// Produtos mais vendidos
print("\n=== PRODUTOS MAIS VENDIDOS ===");
db.datalake.aggregate([
  {
    $match: {
      evento: "pedido_insert",
      timestamp: { $gte: hoje }
    }
  },
  { $unwind: "$pedido.itens" },
  {
    $group: {
      _id: "$pedido.itens.nome",
      quantidadeVendida: { $sum: "$pedido.itens.quantidade" },
      faturamento: { $sum: { $multiply: ["$pedido.itens.preco", "$pedido.itens.quantidade"] } }
    }
  },
  { $sort: { quantidadeVendida: -1 } },
  { $limit: 5 }
]).forEach((produto, index) => {
  const posicao = index + 1;
  print(`${posicao}º ${produto._id} - ${produto.quantidadeVendida} unidades - R$ ${produto.faturamento.toFixed(2)}`);
});
```

## Cenário 5: Monitoramento de Performance

### Objetivo
Monitorar a performance do sistema de Change Streams e identificar gargalos.

### Simulação

```javascript
// 1. Inserir vários pedidos rapidamente (simular carga)
print("🚀 Simulando carga do sistema...");

const clientes = db.clientes.find().toArray();
const itens = db.itens.find().toArray();

for (let i = 0; i < 10; i++) {
  const cliente = clientes[Math.floor(Math.random() * clientes.length)];
  const item = itens[Math.floor(Math.random() * itens.length)];
  
  db.pedidos.insertOne({
    clienteId: cliente._id,
    dataPedido: new Date(),
    itens: [{
      itemId: item._id,
      nome: item.nome,
      preco: item.preco,
      quantidade: Math.floor(Math.random() * 3) + 1
    }],
    total: item.preco * (Math.floor(Math.random() * 3) + 1)
  });
  
  // Pequena pausa para simular tráfego real
  sleep(100);
}

print("✅ Carga simulada concluída");

// 2. Análise de performance do Data Lake
print("\n=== ANÁLISE DE PERFORMANCE ===");

// Latência entre operação e processamento
const latenciasComProcessamento = db.datalake.aggregate([
  {
    $match: {
      evento: "pedido_insert",
      timestamp: { $gte: new Date(Date.now() - 60000) } // Último minuto
    }
  },
  {
    $addFields: {
      latencia: {
        $subtract: ["$metadados.processadoEm", "$timestamp"]
      }
    }
  },
  {
    $group: {
      _id: null,
      latenciaMedia: { $avg: "$latencia" },
      latenciaMaxima: { $max: "$latencia" },
      latenciaMinima: { $min: "$latencia" },
      totalEventos: { $sum: 1 }
    }
  }
]).toArray()[0];

if (latenciasComProcessamento) {
  print(`⚡ Eventos processados: ${latenciasComProcessamento.totalEventos}`);
  print(`📊 Latência média: ${latenciasComProcessamento.latenciaMedia.toFixed(2)}ms`);
  print(`⏱️  Latência máxima: ${latenciasComProcessamento.latenciaMaxima}ms`);
  print(`🚀 Latência mínima: ${latenciasComProcessamento.latenciaMinima}ms`);
}

// Volume de eventos por minuto
const eventosUltimos5Min = db.datalake.aggregate([
  {
    $match: {
      timestamp: { $gte: new Date(Date.now() - 300000) } // Últimos 5 minutos
    }
  },
  {
    $group: {
      _id: {
        minuto: {
          $dateToString: {
            format: "%Y-%m-%d %H:%M",
            date: "$timestamp"
          }
        }
      },
      eventos: { $sum: 1 }
    }
  },
  { $sort: { "_id.minuto": 1 } }
]);

print("\n📈 EVENTOS POR MINUTO:");
eventosUltimos5Min.forEach(stat => {
  print(`${stat._id.minuto}: ${stat.eventos} eventos`);
});
```

## Como Executar os Cenários

1. **Inicie o Data Lake Processor**:
   ```bash
   make run
   ```

2. **Execute um cenário específico**:
   ```bash
   # Copiar o código do cenário desejado e colar no mongo shell
   docker exec -it mongodb-primary mongosh --username admin --password admin123 --authenticationDatabase admin datalake_demo
   ```

3. **Observe os logs em tempo real** no terminal do processor

4. **Verifique os resultados** com:
   ```bash
   make check-datalake
   ```

## Objetivos de Aprendizado

✅ **Real-time Processing**: Eventos são processados instantaneamente  
✅ **Business Intelligence**: Métricas calculadas em tempo real  
✅ **Audit Trail**: Histórico completo de mudanças  
✅ **Performance Monitoring**: Análise de latência e throughput  
✅ **Data Denormalization**: Otimização para consultas analíticas  
✅ **Event-Driven Architecture**: Processamento baseado em eventos