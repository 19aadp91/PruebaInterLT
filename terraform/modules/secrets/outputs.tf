output "db_connection_secret_arn" {
  description = "ARN del secreto de la cadena de conexion a la base de datos"
  value       = aws_secretsmanager_secret.db_connection.arn
}

output "jwt_secret_arn" {
  description = "ARN del secreto de la llave JWT"
  value       = aws_secretsmanager_secret.jwt_key.arn
}

output "secrets_arns" {
  description = "Lista combinada de ARNs de secretos"
  value       = [
    aws_secretsmanager_secret.db_connection.arn,
    aws_secretsmanager_secret.jwt_key.arn
  ]
}
