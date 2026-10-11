output "db_endpoint" {
  description = "Punto de enlace (Endpoint) de conexión a SQL Server"
  value       = aws_db_instance.sqlserver.endpoint
}

output "db_address" {
  description = "Dirección host de la instancia SQL Server"
  value       = aws_db_instance.sqlserver.address
}

output "db_port" {
  description = "Puerto de escucha de SQL Server (1433)"
  value       = aws_db_instance.sqlserver.port
}

output "db_instance_id" {
  description = "ID del recurso RDS creado"
  value       = aws_db_instance.sqlserver.id
}
