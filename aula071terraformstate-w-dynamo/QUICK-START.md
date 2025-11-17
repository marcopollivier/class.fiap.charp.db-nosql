# Guia Rápido - Bucket S3 com Terraform

## Setup Inicial (Uma vez apenas)

```bash
# 1. Configurar AWS CLI
aws configure
# Inserir: Access Key, Secret Key, Region (us-east-1), Output (json)

# 2. Criar backend automaticamente
make create-backend

# 3. Copiar e configurar variáveis
cp terraform.tfvars.example terraform.tfvars
# Editar terraform.tfvars com seu bucket único

# 4. Inicializar Terraform
make init
```

## Fluxo de Desenvolvimento

```bash
# Validar e planejar
make dev          # Equivale a: fmt + validate + plan

# Aplicar mudanças (cria o bucket S3)
make apply

# Ver outputs (nome do bucket criado)
make output

# Ver estado
make show
```

## O que será criado

- **Bucket S3**: `s3-teste-{project-name}-{random-id}`
- **Configurações**: Versionamento, criptografia, acesso privado

## Testando o bucket criado

```bash
# Ver informações do bucket
terraform output s3_bucket_name

# Listar arquivos no bucket (vazio inicialmente)
aws s3 ls s3://$(terraform output -raw s3_bucket_name)/

# Upload de um arquivo teste
echo "Olá, mundo!" > teste.txt
aws s3 cp teste.txt s3://$(terraform output -raw s3_bucket_name)/

# Verificar upload
aws s3 ls s3://$(terraform output -raw s3_bucket_name)/
```

## Comandos Terraform Diretos

```bash
# Comandos básicos
terraform plan
terraform apply
terraform destroy
terraform output
terraform show
terraform state list
```

## Limpeza

```bash
# Esvaziar bucket antes de destruir
aws s3 rm s3://$(terraform output -raw s3_bucket_name) --recursive

# Destruir infraestrutura
make destroy
```