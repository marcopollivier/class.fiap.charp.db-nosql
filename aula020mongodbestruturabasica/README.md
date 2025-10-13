# MongoDB com Dados Iniciais - Aula 020

Este setup configura um MongoDB que **sempre** inicia com dados limpos e pré-populados usando **Makefile** para automação e **pasta local** para dados.

## 🎯 Características

- **Sempre limpo**: Limpa e recria dados a cada inicialização
- **Dados pré-populados**: Cria automaticamente o banco `pedidos` com collections iniciais
- **Pasta local**: Dados armazenados em `./data/db` (visível no sistema de arquivos)
- **Makefile simples**: Comandos padronizados e limpeza automática
- **Controle de versão**: Pasta versionada mas arquivos do MongoDB ignorados

## 📋 Dados Iniciais Criados

### Database: `pedidos`

#### Collection: `clientes`

```javascript
{
  nome: "João Silva",
  email: "joao@exemplo.com"
}
```

#### Collection: `pedidos`

```javascript
{
  cliente: "João Silva",
  data: new Date(),
  itens: [
    { produto: "Notebook", quantidade: 1, preco: 4500 },
    { produto: "Mouse", quantidade: 1, preco: 150 }
  ],
  total: 4650
}
```

## 🚀 Comandos Disponíveis

### Comandos Principais

```bash
make up              # Iniciar MongoDB com dados limpos
make down            # Parar MongoDB e limpar dados
```

### Verificar dados manualmente

```bash
# Conectar no MongoDB shell
docker exec -it aula02-mongodb mongosh -u admin -p password123 --authenticationDatabase admin pedidos

# Verificar collections
db.clientes.find()
db.pedidos.find()
```

## 📁 Estrutura de Arquivos

```text
aula020mongodbestruturabasica/
├── Makefile                    # Comandos de automação
├── docker-compose.yml          # Configuração do MongoDB
├── init-mongo.js              # Script de inicialização com dados
├── README.md                  # Esta documentação
└── data/
    └── db/
        ├── .gitkeep           # Mantém pasta no git
        └── [arquivos MongoDB] # Arquivos gerados (ignorados pelo git)
```

## 🔧 Configuração

- **Porta**: 27017
- **Usuário admin**: admin
- **Senha admin**: password123
- **Database inicial**: pedidos
- **Dados locais**: `./data/db/`

## 💡 Como Funciona

1. **make up**:
   - Limpa pasta `./data/db/*`
   - Recria a pasta
   - Inicia container MongoDB
   - Executa script de inicialização automático

2. **make down**:
   - Para o container
   - Executa limpeza completa

3. **Controle de versão**:
   - `.gitkeep` mantém estrutura da pasta
   - Arquivos do MongoDB são ignorados pelo `.gitignore`

## ⚠️ Importante

- **Dados locais**: Armazenados em `./data/db/` e visíveis no seu sistema
- **Sempre recria**: Perfeito para desenvolvimento e testes
- **Script de init**: Só executa na primeira criação do container
- **Não commitado**: Arquivos do MongoDB não vão para o git
