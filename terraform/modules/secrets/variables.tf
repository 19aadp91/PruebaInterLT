variable "project_name" {
  description = "Nombre del proyecto"
  type        = string
}

variable "environment" {
  description = "Ambiente de despliegue"
  type        = string
}

variable "db_host" {
  description = "Host o dirección de RDS SQL Server"
  type        = string
}

variable "db_port" {
  description = "Puerto de SQL Server"
  type        = number
  default     = 1433
}

variable "db_name" {
  description = "Nombre de la base de datos"
  type        = string
  default     = "StudentRegistrationDb"
}

variable "db_username" {
  description = "Usuario de la base de datos"
  type        = string
}

variable "db_password" {
  description = "Contraseña de la base de datos"
  type        = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "Clave criptográfica para firma de tokens JWT (mínimo 256 bits)"
  type        = string
  sensitive   = true
  default     = "SuperSecretKeyForInterRapidisimoExamSecureSigning2026!ProdKey"
}
