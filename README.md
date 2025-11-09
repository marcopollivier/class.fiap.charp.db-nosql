# NoSQL na Prática com .NET

> Material didático para pós-graduação FIAP - Banco de Dados NoSQL com .NET 9

Este repositório é uma jornada prática pelo mundo NoSQL, onde cada pasta representa uma aula com exemplos reais que você pode executar e modificar.

## 🎯 Por que este curso?

No mundo real, escolher entre SQL e NoSQL não é sobre "melhor ou pior", mas sobre **contexto**. Aqui você vai entender quando, como e por que usar cada tecnologia através de exemplos práticos em .NET.

## 📚 Jornada de Aprendizado

### 🚀 Aula 1: Descobrindo o NoSQL

**Pasta: `aula010introducao/`**

"Afinal, por que não usar SQL para tudo?" - Aqui você vai ver na prática as diferenças fundamentais através de um sistema de pedidos implementado em SQL Server e MongoDB lado a lado.

**O que você vai aprender:**

- Quando NoSQL faz sentido (e quando não faz)
- As 4 famílias NoSQL com exemplos reais
- CAP Theorem aplicado no dia a dia
- Comparação prática: mesmo sistema, bancos diferentes

### 🍃 Aula 2: MongoDB - Primeiros Passos

**Pasta: `aula020mongodbestruturabasica/`**

"Como funciona um banco de documentos?" - Setup completo do MongoDB com dados pré-carregados e automação via Makefile. Você vai entender a estrutura básica sem complicação.

**O que você vai aprender:**

- Setup profissional com Docker
- Conceitos: documentos, coleções, BSON
- Automação de ambiente de desenvolvimento
- Dados persistentes e versionamento

### ⚡ Aula 3: MongoDB Avançado

**Pasta: `aula030mongoavancado/`** *(em desenvolvimento)*

**Próximos tópicos:**

- Modelagem avançada (embedded vs referenced)
- Aggregation Framework para análises
- Transações e consistência
- Change Streams (eventos em tempo real)
- Sharding e Replica Sets

### 🚀 Aulas Futuras

- **Aula 4**: Redis - Cache inteligente e estruturas de dados
- **Aula 5**: DynamoDB - NoSQL serverless da AWS
- **Aula 6**: DynamoDB Avançado - Padrões e performance
- **Aula 7**: Comparação e Projeto Final - Juntando tudo

## 🛠️ Como Começar

### Pré-requisitos

- .NET SDK 9.0
- Docker (ou Podman)
- VS Code ou Visual Studio 2022
- Git

### Executando uma Aula

```bash
# 1. Clone o repositório
git clone <repository-url>

# 2. Navegue para a aula desejada
cd aula010introducao/

# 3. Se houver Makefile, use-o (mais simples)
make up

# 4. Ou use Docker Compose diretamente
docker-compose up -d

# 5. Para projetos .NET
dotnet restore
dotnet run
```

### Comandos Úteis

**Com Makefile (projetos automatizados):**

```bash
make up      # Inicia tudo
make down    # Para e limpa
```

**Com Docker Compose:**

```bash
docker-compose up -d    # Sobe os serviços
docker-compose down     # Para os serviços
```

## 🎯 Dicas de Estudo

1. **Execute os exemplos**: Não só leia, rode os códigos
2. **Compare as abordagens**: Veja as diferenças entre SQL e NoSQL nos mesmos cenários
3. **Modifique os exemplos**: Quebre e conserte para entender melhor
4. **Use os logs**: Analise as queries geradas pelos ORMs/drivers

## 📖 Estrutura de Numeração

- `aula01x` = Aula 1 com variações
- `aula02x` = Aula 2 com variações  
- `aula03x` = Aula 3 com variações

Cada pasta é independente e pode ser executada isoladamente.
