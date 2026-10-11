output "alb_security_group_id" {
  description = "ID del Security Group para el ALB"
  value       = aws_security_group.alb.id
}

output "ecs_security_group_id" {
  description = "ID del Security Group para ECS Fargate"
  value       = aws_security_group.ecs.id
}

output "rds_security_group_id" {
  description = "ID del Security Group para RDS SQL Server"
  value       = aws_security_group.rds.id
}

output "ecs_execution_role_arn" {
  description = "ARN del rol de ejecucion de tareas ECS"
  value       = aws_iam_role.ecs_execution_role.arn
}

output "ecs_task_role_arn" {
  description = "ARN del rol de aplicacion de tareas ECS"
  value       = aws_iam_role.ecs_task_role.arn
}
