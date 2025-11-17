# Informações da conta AWS
output "aws_account_id" {
  description = "ID da conta AWS"
  value       = data.aws_caller_identity.current.account_id
}

output "aws_region" {
  description = "Região AWS utilizada"
  value       = data.aws_region.current.name
}

# Informações do S3 Bucket
output "s3_bucket_name" {
  description = "Nome do bucket S3 criado"
  value       = aws_s3_bucket.s3_teste.bucket
}

output "s3_bucket_arn" {
  description = "ARN do bucket S3 criado"
  value       = aws_s3_bucket.s3_teste.arn
}

output "s3_bucket_domain_name" {
  description = "Domain name do bucket S3"
  value       = aws_s3_bucket.s3_teste.bucket_domain_name
}

# Resumo da infraestrutura criada
output "infrastructure_summary" {
  description = "Resumo da infraestrutura criada"
  value = {
    project_name = var.project_name
    environment  = var.environment
    aws_region   = data.aws_region.current.name
    s3_bucket    = aws_s3_bucket.s3_teste.bucket
    created_at   = timestamp()
  }
}

# Comandos úteis para próximos passos
output "next_steps_commands" {
  description = "Comandos úteis para próximos passos"
  value = {
    "list_resources" = "terraform state list"
    "show_s3_bucket" = "terraform state show aws_s3_bucket.s3_teste"
    "aws_s3_list"    = "aws s3 ls s3://${aws_s3_bucket.s3_teste.bucket}/"
    "aws_s3_upload"  = "aws s3 cp arquivo.txt s3://${aws_s3_bucket.s3_teste.bucket}/"
  }
}
