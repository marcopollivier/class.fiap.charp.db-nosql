# Configure o provider AWS
provider "aws" {
  region = var.aws_region

  # Tags padrão aplicadas a todos os recursos
  default_tags {
    tags = {
      Project     = var.project_name
      Environment = var.environment
      ManagedBy   = "Terraform"
      Course      = "FIAP-NoSQL"
    }
  }
}

# Data source para obter dados da conta AWS atual
data "aws_caller_identity" "current" {}

data "aws_region" "current" {}

# Bucket S3 simples para demonstração
resource "aws_s3_bucket" "s3_teste" {
  bucket = "s3-teste-${var.project_name}-${random_id.bucket_suffix.hex}"

  tags = {
    Name        = "s3-teste"
    Purpose     = "Exemplo simples de S3 com Terraform"
    Environment = var.environment
  }
}

# Random ID para tornar o bucket único globalmente
resource "random_id" "bucket_suffix" {
  byte_length = 4
}

# Configuração de versionamento do bucket
resource "aws_s3_bucket_versioning" "s3_teste" {
  bucket = aws_s3_bucket.s3_teste.id
  versioning_configuration {
    status = "Enabled"
  }
}

# Configuração de criptografia do bucket
resource "aws_s3_bucket_server_side_encryption_configuration" "s3_teste" {
  bucket = aws_s3_bucket.s3_teste.id

  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
  }
}

# Bloquear acesso público ao bucket (boa prática de segurança)
resource "aws_s3_bucket_public_access_block" "s3_teste" {
  bucket = aws_s3_bucket.s3_teste.id

  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}
