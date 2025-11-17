# Exemplo de uso com LocalStack (Ambiente Local)

Este arquivo demonstra como testar o Terraform localmente usando LocalStack,
que simula serviços AWS em containers Docker.

## Configuração LocalStack

### 1. Subir LocalStack

```bash
docker-compose up -d localstack
```

### 2. Verificar se está funcionando

```bash
# Verificar se LocalStack está rodando
curl http://localhost:4566/health

# Listar serviços disponíveis
aws --endpoint-url=http://localhost:4566 s3 ls
```

### 3. Configurar Terraform para LocalStack

Crie um arquivo `terraform.localstack.tf` para override:

```hcl
# Override do provider para LocalStack
terraform {
  backend "s3" {
    endpoint                    = "http://localhost:4566"
    bucket                      = "terraform-state-local"
    key                         = "terraform.tfstate"
    region                      = "us-east-1"
    dynamodb_endpoint          = "http://localhost:4566"
    dynamodb_table             = "terraform-state-lock"
    skip_credentials_validation = true
    skip_metadata_api_check    = true
    skip_requesting_account_id  = true
    force_path_style           = true
  }
}

provider "aws" {
  region                      = "us-east-1"
  access_key                  = "test"
  secret_key                  = "test"
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    s3       = "http://localhost:4566"
    dynamodb = "http://localhost:4566"
    ec2      = "http://localhost:4566"
  }
}
```

### 4. Criar recursos necessários no LocalStack

```bash
# Criar bucket
aws --endpoint-url=http://localhost:4566 s3 mb s3://terraform-state-local

# Criar tabela DynamoDB
aws --endpoint-url=http://localhost:4566 dynamodb create-table \
  --table-name terraform-state-lock \
  --attribute-definitions AttributeName=LockID,AttributeType=S \
  --key-schema AttributeName=LockID,KeyType=HASH \
  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5
```

### 5. Executar Terraform

```bash
# Inicializar
terraform init

# Planejar
terraform plan

# Aplicar
terraform apply
```

## Comandos Úteis LocalStack

```bash
# Ver buckets S3
aws --endpoint-url=http://localhost:4566 s3 ls

# Ver conteúdo do bucket state
aws --endpoint-url=http://localhost:4566 s3 ls s3://terraform-state-local

# Ver tabelas DynamoDB
aws --endpoint-url=http://localhost:4566 dynamodb list-tables

# Ver VPCs criadas
aws --endpoint-url=http://localhost:4566 ec2 describe-vpcs
```

## Vantagens do LocalStack

1. **Desenvolvimento offline**: Não precisa de conexão com AWS
2. **Sem custos**: Testes gratuitos localmente
3. **Rapidez**: Mais rápido que AWS real
4. **Segurança**: Não risca afetar recursos reais
5. **CI/CD**: Ideal para pipelines automatizados

## Limitações do LocalStack

1. **Funcionalidades limitadas**: Nem todos os recursos AWS estão disponíveis
2. **Comportamento diferente**: Pode ter pequenas diferenças da AWS real
3. **Performance**: Para cargas pesadas, AWS real é melhor
4. **Versão gratuita limitada**: Algumas features são pagas

## Limpeza

```bash
# Parar LocalStack
docker-compose down

# Remover volumes (limpar dados)
docker-compose down -v
```

---

**Nota**: Este é apenas um ambiente de desenvolvimento/teste.
Para produção, sempre use AWS real com todas as configurações de segurança adequadas.
