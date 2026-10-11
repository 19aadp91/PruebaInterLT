variable "project_name" {
  description = "Nombre del proyecto"
  type        = string
}

variable "environment" {
  description = "Ambiente de despliegue"
  type        = string
}

variable "isolated_subnet_ids" {
  description = "IDs de las subredes aisladas para el DB Subnet Group"
  type        = list(string)
}

variable "rds_security_group_id" {
  description = "ID del Security Group autorizado para RDS"
  type        = string
}

variable "db_instance_class" {
  description = "Clase de instancia para Amazon RDS SQL Server"
  type        = string
  default     = "db.t3.small"
}

variable "allocated_storage" {
  description = "Espacio de almacenamiento asignado en GB"
  type        = number
  default     = 20
}

variable "multi_az" {
  description = "Habilitar alta disponibilidad Multi-AZ"
  type        = bool
  default     = false
}

variable "db_username" {
  description = "Usuario administrador de la base de datos SQL Server"
  type        = string
  default     = "sa_admin"
}

variable "db_password" {
  description = "Contraseña de la base de datos (Inyectada desde variables de entorno o Secrets Manager)"
  type        = string
  sensitive   = true
}

variable "db_name" {
  description = "Nombre inicial de la base de datos"
  type        = string
  default     = "StudentRegistrationDb"
}
