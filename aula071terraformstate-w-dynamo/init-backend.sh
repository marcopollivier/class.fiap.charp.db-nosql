# Backend Initialization Script
# Este script inicializa o Terraform com backend remoto

#!/bin/bash

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para log
log() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verificar se as variáveis estão definidas
if [ -z "$1" ] || [ -z "$2" ]; then
    echo "Uso: $0 <bucket-name> <table-name> [region]"
    echo "Exemplo: $0 terraform-state-fiap-nosql-joao terraform-state-lock us-east-1"
    exit 1
fi

BUCKET_NAME=$1
TABLE_NAME=$2
AWS_REGION=${3:-us-east-1}

log "Inicializando backend do Terraform..."
log "Bucket: $BUCKET_NAME"
log "Tabela: $TABLE_NAME"
log "Região: $AWS_REGION"

# Verificar se aws cli está instalado
if ! command -v aws &> /dev/null; then
    error "AWS CLI não está instalado"
    exit 1
fi

# Verificar se terraform está instalado
if ! command -v terraform &> /dev/null; then
    error "Terraform não está instalado"
    exit 1
fi

# Verificar credenciais AWS
log "Verificando credenciais AWS..."
if ! aws sts get-caller-identity &> /dev/null; then
    error "Credenciais AWS não configuradas. Execute 'aws configure'"
    exit 1
fi

ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
log "Conectado na conta AWS: $ACCOUNT_ID"

# Verificar se o bucket existe, se não, criar
log "Verificando bucket S3..."
if ! aws s3 ls "s3://$BUCKET_NAME" &> /dev/null; then
    log "Criando bucket S3: $BUCKET_NAME"
    aws s3 mb "s3://$BUCKET_NAME" --region $AWS_REGION
    
    log "Habilitando versionamento..."
    aws s3api put-bucket-versioning \
        --bucket $BUCKET_NAME \
        --versioning-configuration Status=Enabled
    
    log "Configurando criptografia..."
    aws s3api put-bucket-encryption \
        --bucket $BUCKET_NAME \
        --server-side-encryption-configuration '{
            "Rules": [
                {
                    "ApplyServerSideEncryptionByDefault": {
                        "SSEAlgorithm": "AES256"
                    }
                }
            ]
        }'
else
    log "Bucket S3 já existe: $BUCKET_NAME"
fi

# Verificar se a tabela DynamoDB existe, se não, criar
log "Verificando tabela DynamoDB..."
if ! aws dynamodb describe-table --table-name $TABLE_NAME --region $AWS_REGION &> /dev/null; then
    log "Criando tabela DynamoDB: $TABLE_NAME"
    aws dynamodb create-table \
        --table-name $TABLE_NAME \
        --attribute-definitions AttributeName=LockID,AttributeType=S \
        --key-schema AttributeName=LockID,KeyType=HASH \
        --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
        --region $AWS_REGION
    
    log "Aguardando tabela ficar ativa..."
    aws dynamodb wait table-exists --table-name $TABLE_NAME --region $AWS_REGION
else
    log "Tabela DynamoDB já existe: $TABLE_NAME"
fi

# Verificar se terraform.tfvars existe
if [ ! -f terraform.tfvars ]; then
    warn "Arquivo terraform.tfvars não encontrado"
    log "Criando terraform.tfvars a partir do template..."
    cp terraform.tfvars.example terraform.tfvars
    
    # Substituir o nome do bucket no arquivo
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        sed -i '' "s/terraform-state-fiap-nosql-SEU-NOME-UNICO/$BUCKET_NAME/g" terraform.tfvars
    else
        # Linux
        sed -i "s/terraform-state-fiap-nosql-SEU-NOME-UNICO/$BUCKET_NAME/g" terraform.tfvars
    fi
    
    warn "IMPORTANTE: Revise o arquivo terraform.tfvars antes de continuar"
fi

# Inicializar Terraform
log "Inicializando Terraform com backend remoto..."
terraform init \
    -backend-config="bucket=$BUCKET_NAME" \
    -backend-config="key=terraform.tfstate" \
    -backend-config="region=$AWS_REGION" \
    -backend-config="dynamodb_table=$TABLE_NAME" \
    -backend-config="encrypt=true"

log "✅ Backend inicializado com sucesso!"
log ""
log "Próximos passos:"
log "1. Revise o arquivo terraform.tfvars"
log "2. Execute: terraform plan"
log "3. Execute: terraform apply"
log ""
log "Ou use o Makefile:"
log "make plan"
log "make apply"