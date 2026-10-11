variable "project_name" {
  description = "Nombre del proyecto"
  type        = string
}

variable "environment" {
  description = "Ambiente de despliegue"
  type        = string
}

variable "alb_dns_name" {
  description = "Nombre DNS del Application Load Balancer para proxy inverso de API"
  type        = string
  default     = ""
}
