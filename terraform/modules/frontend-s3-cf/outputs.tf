output "s3_bucket_name" {
  description = "Nombre del bucket S3 de almacenamiento de assets"
  value       = aws_s3_bucket.frontend.id
}

output "s3_bucket_arn" {
  description = "ARN del bucket S3"
  value       = aws_s3_bucket.frontend.arn
}

output "cloudfront_domain_name" {
  description = "Dominio público HTTPS de CloudFront para acceder a la aplicación web"
  value       = aws_cloudfront_distribution.cdn.domain_name
}

output "cloudfront_distribution_id" {
  description = "ID de la distribución CloudFront para invalidación de caché en pipelines CD"
  value       = aws_cloudfront_distribution.cdn.id
}
