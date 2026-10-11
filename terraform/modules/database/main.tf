# DB Subnet Group en subredes aisladas
resource "aws_db_subnet_group" "rds" {
  name        = "${var.project_name}-${var.environment}-db-subnet-group"
  description = "Subredes aisladas para SQL Server sin conectividad a internet"
  subnet_ids  = var.isolated_subnet_ids

  tags = {
    Name        = "${var.project_name}-${var.environment}-db-subnet-group"
    Environment = var.environment
  }
}

# Instancia RDS SQL Server
resource "aws_db_instance" "sqlserver" {
  identifier        = "${var.project_name}-${var.environment}-db"
  engine            = "sqlserver-ex" # Express Edition para MVP / sqlserver-se para Standard
  engine_version    = "15.00.4385.2.v1"
  instance_class    = var.db_instance_class
  allocated_storage = var.allocated_storage
  storage_type      = "gp3"
  storage_encrypted = true

  username = var.db_username
  password = var.db_password
  port     = 1433

  db_subnet_group_name   = aws_db_subnet_group.rds.name
  vpc_security_group_ids = [var.rds_security_group_id]
  publicly_accessible    = false
  multi_az               = var.multi_az

  auto_minor_version_upgrade = true
  backup_retention_period   = 7
  skip_final_snapshot       = true # Ajustar a false para ambientes de producción con retención estricta
  deletion_protection       = false

  tags = {
    Name        = "${var.project_name}-${var.environment}-sqlserver"
    Environment = var.environment
  }
}
