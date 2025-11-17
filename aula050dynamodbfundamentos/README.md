# Aula 5: DynamoDB - Fundamentos e Modelagem

> **Objetivo**: Dominar os conceitos fundamentais do DynamoDB, modelagem de dados para NoSQL chave-valor e boas práticas para AWS.

## 🎯 O que Você Vai Aprender

- Conceitos: Partition Key e Sort Key
- Modelagem single-table vs multi-table
- Índices: GSI (Global Secondary Index) e LSI (Local Secondary Index)
- CRUD operations com AWS SDK para .NET
- Padrões de acesso e hot spots
- Estratégias de particionamento

## 🚀 Como Executar

```bash
# 1. Subir DynamoDB Local
docker-compose up -d

# 2. Configurar AWS CLI (para DynamoDB local)
aws configure set aws_access_key_id "fakekey"
aws configure set aws_secret_access_key "fakesecret"
aws configure set region "us-east-1"

# 3. Restaurar dependências
dotnet restore

# 4. Executar exemplos
dotnet run
```

## 📚 Documentação Organizada

Esta aula está estruturada em módulos progressivos para facilitar o aprendizado. Consulte a [documentação completa na pasta doc/](./doc/).

### 📖 Conteúdo Fundamental

1. **[O que é o DynamoDB](./doc/01-o-que-e-dynamodb.md)**
   - Conceitos introdutórios e comparações
   - Casos de uso e características principais

2. **[Características Básicas](./doc/02-caracteristicas-basicas.md)**
   - Arquitetura interna e particionamento
   - Modelos de billing e capacidade

3. **[Partition Key e Sort Key](./doc/03-partition-key-sort-key.md)**
   - Fundamentos de modelagem de chaves
   - Padrões de consulta e distribuição

4. **[Índices GSI e LSI](./doc/04-indices-gsi-lsi.md)**
   - Índices secundários para acesso alternativo
   - Estratégias de projeção e performance

5. **[Estratégias de Modelagem](./doc/05-estrategias-modelagem.md)**
   - Single Table vs Multiple Tables
   - Padrões avançados e migração

6. **[Hot Spots e Distribuição](./doc/06-hot-spots-distribuicao.md)**
   - Prevenção de gargalos de performance
   - Técnicas de distribuição uniforme

### 🎯 Como Estudar

- **Iniciantes**: Siga a ordem sequencial (módulos 1-6)
- **Intermediários**: Foque em modelagem (módulos 3-5)  
- **Avançados**: Concentre-se em otimização (módulo 6)

### Próximos Passos

- Setup DynamoDB Local com Docker
- Implementação prática em .NET
- Exercícios com dados do e-commerce padrão

## 🔗 Links Relacionados

- [Aula 4.0: Redis](../aula040redis/)
- [Aula 6.0: DynamoDB Avançado](../aula060dynamodbavancado/)
