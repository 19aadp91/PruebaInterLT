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

variable "public_subnet_ids" {
  description = "Subredes publicas para el Application Load Balancer"
  type        = list(string)
}

variable "private_subnet_ids" {
  description = "Subredes privadas de computo para las tareas ECS Fargate"
  type        = list(string)
}

variable "alb_security_group_id" {
  description = "Security Group del ALB"
  type        = string
}

variable "ecs_security_group_id" {
  description = "Security Group de las tareas ECS Fargate"
  type        = string
}

variable "ecs_execution_role_arn" {
  description = "ARN del rol de ejecucion IAM de ECS"
  type        = string
}

variable "ecs_task_role_arn" {
  description = "ARN del rol de tarea IAM de ECS"
  type        = string
}

variable "container_image" {
  description = "URI de la imagen en Amazon ECR o Docker Hub"
  type        = string
  default     = "123456789012.dkr.ecr.us-east-1.amazonaws.com/student-registration-api:latest"
}

variable "container_port" {
  description = "Puerto no privilegiado expuesto por el contenedor"
  type        = number
  default     = 8080
}

variable "task_cpu" {
  description = "Capacidad de CPU para la tarea Fargate"
  type        = number
  default     = 256
}

variable "task_memory" {
  description = "Memoria RAM en MB para la tarea Fargate"
  type        = number
  default     = 512
}

variable "desired_count" {
  description = "Numero deseado de replicas para alta disponibilidad"
  type        = number
  default     = 2
}

variable "db_connection_secret_arn" {
  description = "ARN del secreto de conexion de BD en Secrets Manager"
  type        = string
}

variable "jwt_secret_arn" {
  description = "ARN del secreto de la llave JWT en Secrets Manager"
  type        = string
}
