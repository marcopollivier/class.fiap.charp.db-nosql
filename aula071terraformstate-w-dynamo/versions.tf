terraform {
  required_version = ">= 1.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.1"
    }
  }

  # Backend configuration para remote state
  backend "s3" {
    # bucket será definido na inicialização
    # key será definido na inicialização
    # region será definido na inicialização
    # dynamodb_table será definido na inicialização

    # Configurações de segurança
    encrypt       = true
    versioning    = true
    force_destroy = false
  }
}
