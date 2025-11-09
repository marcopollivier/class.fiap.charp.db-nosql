# Modelagem no MongoDB: Embedded vs Referenced

## Introdução

Uma das decisões mais importantes no design de bancos NoSQL documento-orientados como o MongoDB é escolher entre **embedded documents** (documentos embarcados) e **referenced documents** (documentos referenciados).

## Embedded Documents (Documentos Embarcados)

### Conceito
Os dados relacionados são armazenados dentro do mesmo documento, criando uma estrutura aninhada.

### Quando Usar
- **Relacionamento 1:1**: Dados que sempre são acessados juntos
- **Relacionamento 1:poucos**: Quando a quantidade de subdocumentos é limitada
- **Dados que não mudam com frequência**
- **Consultas que precisam de todos os dados de uma vez**

### Vantagens
- ✅ **Performance**: Uma única consulta retorna todos os dados
- ✅ **Atomicidade**: Operações em um documento são atômicas
- ✅ **Simplicidade**: Menos joins, estrutura mais simples

### Desvantagens
- ❌ **Limite de documento**: MongoDB tem limite de 16MB por documento
- ❌ **Duplicação de dados**: Pode causar inconsistência
- ❌ **Crescimento ilimitado**: Arrays que crescem infinitamente

### Exemplo Prático
```javascript
// Embedded - Pedido com itens embarcados
{
  "_id": ObjectId("..."),
  "numeroPedido": "PED-001",
  "cliente": "João Silva",
  "dataCreacao": ISODate("2024-01-15"),
  "itens": [
    {
      "produto": "Notebook Dell",
      "quantidade": 1,
      "preco": 2500.00
    },
    {
      "produto": "Mouse Logitech",
      "quantidade": 2,
      "preco": 45.00
    }
  ],
  "total": 2590.00
}
```

## Referenced Documents (Documentos Referenciados)

### Conceito
Os dados relacionados são armazenados em documentos separados, conectados através de referências (IDs).

### Quando Usar
- **Relacionamento 1:muitos**: Quando há muitos documentos relacionados
- **Relacionamento muitos:muitos**: Relacionamentos complexos
- **Dados que mudam com frequência**
- **Dados que são consultados independentemente**
- **Necessidade de normalização**

### Vantagens
- ✅ **Flexibilidade**: Melhor para relacionamentos complexos
- ✅ **Normalização**: Evita duplicação de dados
- ✅ **Escalabilidade**: Não há limite de crescimento
- ✅ **Consistência**: Dados centralizados

### Desvantagens
- ❌ **Performance**: Múltiplas consultas ou uso de $lookup
- ❌ **Complexidade**: Mais lógica para manter relacionamentos
- ❌ **Atomicidade**: Operações em múltiplos documentos não são atômicas por padrão

### Exemplo Prático
```javascript
// Referenced - Pedido com referência aos itens
// Coleção: pedidos
{
  "_id": ObjectId("..."),
  "numeroPedido": "PED-001",
  "clienteId": ObjectId("..."),
  "dataCreacao": ISODate("2024-01-15"),
  "itensIds": [
    ObjectId("item1"),
    ObjectId("item2")
  ],
  "total": 2590.00
}

// Coleção: itens
{
  "_id": ObjectId("item1"),
  "produto": "Notebook Dell",
  "quantidade": 1,
  "preco": 2500.00
}
```

## Padrões Híbridos

### Padrão de Subset
Embarca apenas os dados mais frequentemente acessados:

```javascript
{
  "_id": ObjectId("..."),
  "nome": "João Silva",
  "email": "joao@email.com",
  "enderecoPrincipal": {
    "rua": "Rua A, 123",
    "cidade": "São Paulo"
  },
  "enderecosIds": [ObjectId("end1"), ObjectId("end2")]
}
```

### Padrão de Referência Bidirecional
Mantém referências em ambas as direções para otimizar consultas:

```javascript
// Autor
{
  "_id": ObjectId("autor1"),
  "nome": "Autor",
  "livrosIds": [ObjectId("livro1"), ObjectId("livro2")]
}

// Livro
{
  "_id": ObjectId("livro1"),
  "titulo": "Livro 1",
  "autorId": ObjectId("autor1")
}
```

## Diretrizes para Decisão

### Use Embedded quando:
- Relacionamento 1:1 ou 1:poucos
- Dados sempre acessados juntos
- Dados não mudam com frequência
- Atomicidade é crítica

### Use Referenced quando:
- Relacionamento 1:muitos ou muitos:muitos
- Dados consultados independentemente
- Dados mudam com frequência
- Necessita de normalização
- Tamanho do documento pode exceder 16MB

## Considerações de Performance

### Embedded
```javascript
// Uma única consulta
db.pedidos.findOne({"numeroPedido": "PED-001"})
```

### Referenced
```javascript
// Múltiplas consultas ou agregação
db.pedidos.aggregate([
  { $match: {"numeroPedido": "PED-001"} },
  { $lookup: {
    from: "itens",
    localField: "itensIds",
    foreignField: "_id",
    as: "itens"
  }}
])
```

## Exercícios Práticos

1. **E-commerce**: Modele produtos com categorias, avaliações e comentários
2. **Blog**: Modele posts com comentários e tags
3. **Rede Social**: Modele usuários com posts e relacionamentos

## Referências

- [MongoDB Data Modeling](https://docs.mongodb.com/manual/core/data-modeling-introduction/)
- [Schema Design Best Practices](https://www.mongodb.com/developer/products/mongodb/mongodb-schema-design-best-practices/)
