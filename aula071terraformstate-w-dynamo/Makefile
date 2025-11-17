# Makefile para automação dos comandos Terraform
# Uso: make <target>

.PHONY: help init plan apply destroy clean fmt validate show output state-list

# Variáveis padrão
BUCKET_NAME ?= terraform-state-fiap-nosql-$(USER)
TABLE_NAME ?= terraform-state-lock
AWS_REGION ?= us-east-1

# Ajuda
help: ## Mostrar esta ajuda
	@echo "Comandos disponíveis:"
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*?## / {printf "\033[36m%-15s\033[0m %s\n", $$1, $$2}' $(MAKEFILE_LIST)

# Verificar pré-requisitos
check-prereqs: ## Verificar se as ferramentas necessárias estão instaladas
	@echo "Verificando pré-requisitos..."
	@command -v terraform >/dev/null 2>&1 || { echo >&2 "Terraform não está instalado. Aborting."; exit 1; }
	@command -v aws >/dev/null 2>&1 || { echo >&2 "AWS CLI não está instalado. Aborting."; exit 1; }
	@aws sts get-caller-identity >/dev/null 2>&1 || { echo >&2 "AWS não está configurado. Execute 'aws configure'. Aborting."; exit 1; }
	@echo "✅ Todos os pré-requisitos atendidos"

# Criar recursos AWS necessários para o backend
create-backend: check-prereqs ## Criar bucket S3 e tabela DynamoDB para o backend
	@echo "Criando bucket S3: $(BUCKET_NAME)"
	@aws s3 mb s3://$(BUCKET_NAME) --region $(AWS_REGION) || echo "Bucket já existe ou erro na criação"
	@echo "Habilitando versionamento no bucket..."
	@aws s3api put-bucket-versioning --bucket $(BUCKET_NAME) --versioning-configuration Status=Enabled
	@echo "Criando tabela DynamoDB: $(TABLE_NAME)"
	@aws dynamodb create-table \
		--table-name $(TABLE_NAME) \
		--attribute-definitions AttributeName=LockID,AttributeType=S \
		--key-schema AttributeName=LockID,KeyType=HASH \
		--provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5 \
		--region $(AWS_REGION) || echo "Tabela já existe ou erro na criação"
	@echo "✅ Backend configurado com sucesso"

# Verificar se terraform.tfvars existe
check-tfvars:
	@if [ ! -f terraform.tfvars ]; then \
		echo "❌ Arquivo terraform.tfvars não encontrado"; \
		echo "Copie terraform.tfvars.example para terraform.tfvars e configure os valores:"; \
		echo "cp terraform.tfvars.example terraform.tfvars"; \
		exit 1; \
	fi

# Inicializar Terraform
init: check-prereqs check-tfvars ## Inicializar Terraform com backend remoto
	@echo "Inicializando Terraform..."
	@terraform init \
		-backend-config="bucket=$(BUCKET_NAME)" \
		-backend-config="key=terraform.tfstate" \
		-backend-config="region=$(AWS_REGION)" \
		-backend-config="dynamodb_table=$(TABLE_NAME)" \
		-backend-config="encrypt=true"
	@echo "✅ Terraform inicializado"

# Planejar mudanças
plan: ## Mostrar plano de execução
	@echo "Gerando plano de execução..."
	@terraform plan -out=tfplan
	@echo "✅ Plano gerado. Execute 'make apply' para aplicar"

# Aplicar mudanças
apply: ## Aplicar mudanças na infraestrutura
	@echo "Aplicando mudanças na infraestrutura..."
	@if [ -f tfplan ]; then \
		terraform apply tfplan; \
	else \
		terraform apply; \
	fi
	@rm -f tfplan
	@echo "✅ Infraestrutura aplicada com sucesso"

# Destruir infraestrutura
destroy: ## Destruir toda a infraestrutura
	@echo "⚠️  ATENÇÃO: Isso irá destruir TODA a infraestrutura!"
	@echo "Digite 'yes' para confirmar ou Ctrl+C para cancelar"
	@read -r CONFIRM
	@terraform destroy
	@echo "✅ Infraestrutura destruída"

# Mostrar outputs
output: ## Mostrar outputs da infraestrutura
	@terraform output

# Mostrar estado atual
show: ## Mostrar estado detalhado da infraestrutura
	@terraform show

# Listar recursos no state
state-list: ## Listar todos os recursos no state
	@terraform state list

# Formatar arquivos
fmt: ## Formatar arquivos Terraform
	@echo "Formatando arquivos Terraform..."
	@terraform fmt -recursive
	@echo "✅ Arquivos formatados"

# Validar configuração
validate: ## Validar configuração Terraform
	@echo "Validando configuração..."
	@terraform validate
	@echo "✅ Configuração válida"

# Limpeza
clean: ## Limpar arquivos temporários
	@echo "Limpando arquivos temporários..."
	@rm -f tfplan
	@rm -f *.log
	@rm -f *.tmp
	@echo "✅ Limpeza concluída"

# Workflow completo para primeira execução
first-run: check-prereqs create-backend init plan ## Workflow completo para primeira execução
	@echo "🎉 Pronto para aplicar! Execute 'make apply' para criar a infraestrutura"

# Comando de desenvolvimento rápido
dev: fmt validate plan ## Workflow de desenvolvimento (format + validate + plan)

# Mostrar informações úteis
info: ## Mostrar informações do ambiente
	@echo "=== Informações do Ambiente ==="
	@echo "AWS Account ID: $$(aws sts get-caller-identity --query Account --output text)"
	@echo "AWS Region: $$(aws configure get region)"
	@echo "AWS User: $$(aws sts get-caller-identity --query Arn --output text)"
	@echo "Terraform Version: $$(terraform version -json | jq -r .terraform_version)"
	@echo "Bucket Name: $(BUCKET_NAME)"
	@echo "DynamoDB Table: $(TABLE_NAME)"
	@echo "================================"

# Verificar backend
check-backend: ## Verificar se o backend está funcionando
	@echo "Verificando bucket S3..."
	@aws s3 ls s3://$(BUCKET_NAME) || echo "❌ Erro ao acessar bucket"
	@echo "Verificando tabela DynamoDB..."
	@aws dynamodb describe-table --table-name $(TABLE_NAME) --region $(AWS_REGION) >/dev/null || echo "❌ Erro ao acessar tabela DynamoDB"
	@echo "✅ Backend verificado"