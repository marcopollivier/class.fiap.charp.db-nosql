# Aula 6: DynamoDB Avançado

> **Objetivo**: Explorar recursos avançados do DynamoDB como streams, transações, TTL e padrões arquiteturais para aplicações serverless.

## 🎯 O que Você Vai Aprender

- Consistência eventual vs forte
- Transações no DynamoDB
- TTL (Time To Live) para dados temporários
- DynamoDB Streams para eventos
- Adaptive Capacity e auto-scaling
- Padrões: CQRS, Event Sourcing
- Integração com Lambda e .NET

## 🚀 Como Executar

```bash
# 1. Subir DynamoDB Local com Streams
docker-compose up -d

# 2. Configurar AWS CLI
aws configure set aws_access_key_id "fakekey"
aws configure set aws_secret_access_key "fakesecret"
aws configure set region "us-east-1"

# 3. Restaurar dependências
dotnet restore

# 4. Executar exemplos avançados
dotnet run
```

## 📚 Tópicos Abordados

### Em Desenvolvimento

*Este conteúdo está sendo preparado e será adicionado em breve.*

### Próximos Passos

- Configuração de DynamoDB Streams
- Implementação de transações
- Padrões de TTL
- Eventos e processamento assíncrono
- Integração com arquitetura serverless

## 🔗 Links Relacionados

- [Aula 5.0: DynamoDB Fundamentos](../aula050dynamodbfundamentos/)
- [Aula 7.0: Comparação e Projeto Final](../aula070comparacao/)
