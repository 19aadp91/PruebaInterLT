# Secreto para Cadena de Conexión a Base de Datos
resource "aws_secretsmanager_secret" "db_connection" {
  name                    = "${var.project_name}/${var.environment}/db-connection-string"
  description             = "Cadena de conexion a Amazon RDS SQL Server para la API"
  recovery_window_in_days = 0

  tags = {
    Environment = var.environment
  }
}

resource "aws_secretsmanager_secret_version" "db_connection_val" {
  secret_id = aws_secretsmanager_secret.db_connection.id
  secret_string = "Server=${var.db_host},${var.db_port};Database=${var.db_name};User Id=${var.db_username};Password=${var.db_password};TrustServerCertificate=True;MultipleActiveResultSets=true;"
}

# Secreto para Llave de Firma de Tokens JWT
resource "aws_secretsmanager_secret" "jwt_key" {
  name                    = "${var.project_name}/${var.environment}/jwt-signing-key"
  description             = "Clave secreta HMAC para generacion y validacion de tokens JWT"
  recovery_window_in_days = 0

  tags = {
    Environment = var.environment
  }
}

resource "aws_secretsmanager_secret_version" "jwt_key_val" {
  secret_id     = aws_secretsmanager_secret.jwt_key.id
  secret_string = var.jwt_secret_key
}
