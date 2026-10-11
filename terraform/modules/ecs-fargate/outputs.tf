output "alb_dns_name" {
  description = "Nombre DNS público del Application Load Balancer para consumir la API"
  value       = aws_lb.api.dns_name
}

output "alb_arn" {
  description = "ARN del Load Balancer"
  value       = aws_lb.api.arn
}

output "ecs_cluster_id" {
  description = "ID del clúster ECS"
  value       = aws_ecs_cluster.main.id
}

output "ecs_service_name" {
  description = "Nombre del servicio ECS"
  value       = aws_ecs_service.api.name
}

output "ecr_repository_url" {
  description = "URI del repositorio Amazon ECR"
  value       = aws_ecr_repository.api.repository_url
}

output "ecr_repository_name" {
  description = "Nombre del repositorio Amazon ECR"
  value       = aws_ecr_repository.api.name
}
