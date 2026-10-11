variable "aws_region" {
  description = "Región de AWS para desplegar la infraestructura"
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Nombre base del proyecto"
  type        = string
  default     = "student-registration"
}

variable "environment" {
  description = "Ambiente de despliegue (dev, qa, prod)"
  type        = string
  default     = "prod"
}

variable "db_password" {
  description = "Contraseña maestra para la base de datos SQL Server"
  type        = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "Clave HMAC para firma de tokens JWT"
  type        = string
  sensitive   = true
}

variable "container_image" {
  description = "URI de la imagen de contenedor Docker para la API"
  type        = string
  default     = "123456789012.dkr.ecr.us-east-1.amazonaws.com/student-registration-api:latest"
}

variable "desired_count" {
  description = "Numero deseado de replicas para las tareas ECS Fargate"
  type        = number
  default     = 1
}
