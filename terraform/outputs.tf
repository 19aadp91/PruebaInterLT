output "frontend_url" {
  description = "URL publica HTTPS de la aplicacion Frontend (CloudFront)"
  value       = "https://${module.frontend.cloudfront_domain_name}"
}

output "api_base_url" {
  description = "URL publica del Application Load Balancer para la API"
  value       = "http://${module.ecs_fargate.alb_dns_name}"
}

output "s3_bucket_name" {
  description = "Nombre del bucket S3 para despliegue de assets de Angular"
  value       = module.frontend.s3_bucket_name
}

output "cloudfront_distribution_id" {
  description = "ID de la distribucion CloudFront para invalidacion de cache"
  value       = module.frontend.cloudfront_distribution_id
}

output "rds_endpoint" {
  description = "Punto de enlace privado de Amazon RDS SQL Server"
  value       = module.database.db_endpoint
}

output "ecr_repository_url" {
  description = "URI del repositorio Amazon ECR"
  value       = module.ecs_fargate.ecr_repository_url
}
