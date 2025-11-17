variable "aws_region" {
  description = "Região da AWS onde os recursos serão criados"
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Nome do projeto para identificação de recursos"
  type        = string
  default     = "fiap-nosql"
}

variable "environment" {
  description = "Ambiente (dev, staging, prod)"
  type        = string
  default     = "dev"
}

# Variáveis para o backend (não usadas no código, apenas documentação)
variable "bucket_name" {
  description = "Nome do bucket S3 para o state do Terraform"
  type        = string
  default     = "terraform-state-fiap-nosql-CHANGE-ME"
}

variable "dynamodb_table" {
  description = "Nome da tabela DynamoDB para state locking"
  type        = string
  default     = "terraform-state-lock"
}
