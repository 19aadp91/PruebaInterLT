variable "project_name" {
  description = "Nombre del proyecto"
  type        = string
}

variable "environment" {
  description = "Ambiente de despliegue"
  type        = string
}

variable "vpc_id" {
  description = "ID de la VPC"
  type        = string
}

variable "secrets_arns" {
  description = "Lista de ARNs de secretos en Secrets Manager autorizados para lectura"
  type        = list(string)
  default     = ["*"]
}
