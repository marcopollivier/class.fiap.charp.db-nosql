# Aula 071 - Terraform State com S3 + DynamoDB

## Objetivo Educacional

Este módulo demonstra o uso do Terraform para criar um **bucket S3 simples** utilizando:

- **S3** para armazenar o state do Terraform (backend remoto)
- **DynamoDB** para state locking (prevenção de conflitos)
- **Exemplo prático** de criação de bucket S3 com boas práticas

## Conceitos Abordados

- Infrastructure as Code (IaC)
- Remote state management
- State locking com DynamoDB
- Criação segura de recursos S3
- Boas práticas de segurança

## O que será criado

- **1 Bucket S3** chamado `s3-teste-{project-name}-{random-id}`
- Bucket com:
  - Versionamento habilitado
  - Criptografia AES256
  - Bloqueio de acesso público
  - Tags organizacionais

## Pré-requisitos

### 1. Instalação do Terraform

**macOS (usando Homebrew):**

```bash
brew tap hashicorp/tap
brew install hashicorp/tap/terraform
```

**Linux:**

```bash
wget -O- https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg
echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list
sudo apt update && sudo apt install terraform
```

Verificar instalação:

```bash
terraform version
```

### 2. Configuração da AWS CLI

**Instalar AWS CLI:**

```bash
# macOS
brew install awscli

# Linux
sudo apt install awscli
```

**Configurar credenciais:**

```bash
aws configure
```

Você precisará de:

- AWS Access Key ID
- AWS Secret Access Key
- Default region (ex: us-east-1)
- Default output format (json)

### 3. Verificar Acesso à AWS

```bash
aws sts get-caller-identity
```

## Configuração Inicial na AWS

### Passo 1: Criar Bucket S3 para State

O bucket S3 deve ser criado **manualmente** antes de usar o Terraform com remote state:

```bash
# Substitua 'seu-nome-unico' por um nome único globalmente
aws s3 mb s3://terraform-state-fiap-nosql-seu-nome-unico --region us-east-1
```

**Habilitar versionamento:**

```bash
aws s3api put-bucket-versioning \
  --bucket terraform-state-fiap-nosql-seu-nome-unico \
  --versioning-configuration Status=Enabled
```

### Passo 2: Criar Tabela DynamoDB para Locking

```bash
aws dynamodb create-table \
  --table-name terraform-state-lock \
  --attribute-definitions AttributeName=LockID,AttributeType=S \
  --key-schema AttributeName=LockID,KeyType=HASH \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
  --region us-east-1
```

## Estrutura do Projeto

```
aula071terraform/
├── README.md
├── main.tf              # Configuração principal
├── variables.tf         # Variáveis de entrada
├── outputs.tf          # Outputs da infraestrutura
├── terraform.tfvars   # Valores das variáveis (não versionar)
└── versions.tf         # Versões do Terraform e providers
```

## Como Executar

### 1. Configurar Variáveis

Copie o arquivo de exemplo e configure suas variáveis:

```bash
cp terraform.tfvars.example terraform.tfvars
```

Edite `terraform.tfvars` com seus valores:

```hcl
bucket_name = "terraform-state-fiap-nosql-SEU-NOME-UNICO"
dynamodb_table = "terraform-state-lock"
aws_region = "us-east-1"
```

### 2. Inicializar Terraform

```bash
terraform init
```

Este comando irá:

- Baixar os providers necessários
- Configurar o backend remoto (S3 + DynamoDB)

### 3. Planejar as Mudanças

```bash
terraform plan
```

Revise as mudanças que serão aplicadas.

### 4. Aplicar a Infraestrutura

```bash
terraform apply
```

Digite `yes` para confirmar.

### 5. Verificar Outputs

```bash
terraform output
```

### 6. Destruir a Infraestrutura (quando necessário)

```bash
terraform destroy
```

⚠️ **ATENÇÃO**: Isso irá remover todos os recursos criados!

## Comandos Úteis

### Verificar State

```bash
# Ver estado atual
terraform show

# Listar recursos no state
terraform state list

# Ver detalhes de um recurso específico
terraform state show aws_vpc.main
```

### Formatação e Validação

```bash
# Formatar arquivos
terraform fmt

# Validar configuração
terraform validate
```

### Logs de Debug

```bash
export TF_LOG=DEBUG
terraform apply
```

## Estrutura da Infraestrutura Criada

Este exemplo cria:

1. **Bucket S3** - `s3-teste-{project-name}-{random-id}`
   - Nome único globalmente
   - Versionamento habilitado
   - Criptografia AES256
   - Bloqueio de acesso público
   - Tags organizacionais

## Boas Práticas Demonstradas

### 1. Remote State

- State armazenado no S3 (centralizado e seguro)
- Versionamento habilitado para histórico
- Criptografia em repouso

### 2. State Locking

- DynamoDB previne execuções simultâneas
- Evita corrupção do state
- Visibilidade de quem está usando

### 3. Organização do Código

- Separação de concerns (main, variables, outputs)
- Versionamento de providers
- Configuração através de variáveis

### 4. Segurança

- Não hardcoding de credenciais
- Uso de IAM roles quando possível
- Princípio do menor privilégio
- Bucket privado por padrão

## Troubleshooting

### Erro: "Backend initialization required"

```bash
terraform init -reconfigure
```

### Erro: "DynamoDB table not found"

Verifique se a tabela foi criada corretamente:

```bash
aws dynamodb describe-table --table-name terraform-state-lock
```

### Erro: "S3 bucket does not exist"

Verifique se o bucket foi criado e o nome está correto:

```bash
aws s3 ls s3://seu-bucket-name
```

### Erro: "Access Denied"

Verifique suas credenciais AWS:

```bash
aws sts get-caller-identity
aws configure list
```

## Próximos Passos

1. Explore outros recursos AWS (EC2, RDS, etc.)
2. Implemente módulos reutilizáveis
3. Configure ambientes múltiplos (dev, prod)
4. Integre com CI/CD pipelines
5. Explore Terraform Cloud/Enterprise

## Recursos Adicionais

- [Terraform AWS Provider](https://registry.terraform.io/providers/hashicorp/aws/latest)
- [AWS CLI Reference](https://awscli.amazonaws.com/v2/documentation/api/latest/index.html)
- [Terraform Best Practices](https://www.terraform.io/docs/cloud/guides/recommended-practices/index.html)

---

## Notas Educacionais

Este exemplo foca em:

- **Simplicidade**: Infraestrutura básica para aprendizado
- **Boas práticas**: Remote state e locking desde o início
- **Segurança**: Configuração adequada de credenciais
- **Reprodutibilidade**: Qualquer aluno pode executar

O objetivo é familiarizar os alunos com IaC usando Terraform, preparando-os para cenários mais complexos em ambientes profissionais.

```json
{
  "_id": 123,
  "title": "Como usar DynamoDB",
  "body": "texto do artigo",
  "embedding": [0.123, -0.044, ...]  // <-- adicionado
}
```
