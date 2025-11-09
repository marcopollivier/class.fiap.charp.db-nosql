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

## 📚 Tópicos Abordados

### Em Desenvolvimento

*Este conteúdo está sendo preparado e será adicionado em breve.*

### Próximos Passos

- Setup DynamoDB Local
- Modelagem de dados
- Implementação CRUD em .NET
- Criação e uso de índices
- Padrões de consulta eficientes

## 🔗 Links Relacionados

- [Aula 4.0: Redis](../aula040redis/)
- [Aula 6.0: DynamoDB Avançado](../aula060dynamodbavancado/)
