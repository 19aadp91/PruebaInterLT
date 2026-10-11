terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  # Configuración recomendada de backend remoto para ambientes empresariales:
  # backend "s3" {
  #   bucket         = "student-registration-terraform-state"
  #   key            = "prod/terraform.tfstate"
  #   region         = "us-east-1"
  #   dynamodb_table = "terraform-locks"
  # }
}

provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      Project     = var.project_name
      Environment = var.environment
      ManagedBy   = "Terraform"
      Owner       = "Inter-Rapidisimo"
    }
  }
}

# 1. Módulo de Red (VPC con 6 subredes en 2 AZs y NAT Gateway)
module "vpc" {
  source = "./modules/vpc"

  project_name = var.project_name
  environment  = var.environment
}

# 2. Módulo de Base de Datos (Amazon RDS SQL Server en subredes aisladas)
module "database" {
  source = "./modules/database"

  project_name          = var.project_name
  environment           = var.environment
  isolated_subnet_ids   = module.vpc.isolated_subnet_ids
  rds_security_group_id = module.security.rds_security_group_id
  db_password           = var.db_password
}

# 3. Módulo de Secretos (AWS Secrets Manager)
module "secrets" {
  source = "./modules/secrets"

  project_name   = var.project_name
  environment    = var.environment
  db_host        = module.database.db_address
  db_port        = module.database.db_port
  db_username    = "sa_admin"
  db_password    = var.db_password
  jwt_secret_key = var.jwt_secret_key
}

# 4. Módulo de Seguridad (Security Groups y Roles IAM)
module "security" {
  source = "./modules/security"

  project_name = var.project_name
  environment  = var.environment
  vpc_id       = module.vpc.vpc_id
  secrets_arns = module.secrets.secrets_arns
}

# 5. Módulo de Cómputo (AWS ECS Fargate y Application Load Balancer)
module "ecs_fargate" {
  source = "./modules/ecs-fargate"

  project_name             = var.project_name
  environment              = var.environment
  vpc_id                   = module.vpc.vpc_id
  public_subnet_ids        = module.vpc.public_subnet_ids
  private_subnet_ids       = module.vpc.private_subnet_ids
  alb_security_group_id    = module.security.alb_security_group_id
  ecs_security_group_id    = module.security.ecs_security_group_id
  ecs_execution_role_arn   = module.security.ecs_execution_role_arn
  ecs_task_role_arn        = module.security.ecs_task_role_arn
  container_image          = var.container_image
  desired_count            = var.desired_count
  db_connection_secret_arn = module.secrets.db_connection_secret_arn
  jwt_secret_arn           = module.secrets.jwt_secret_arn
}

# 6. Módulo de Frontend (Amazon S3 Privado con CloudFront OAC)
module "frontend" {
  source = "./modules/frontend-s3-cf"

  project_name = var.project_name
  environment  = var.environment
  alb_dns_name = module.ecs_fargate.alb_dns_name
}
