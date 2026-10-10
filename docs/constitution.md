# Constitución del Proyecto
1. Stack: ASP.NET Core, Angular Signals y SQL Server; mediador nativo interno y prohibido el paquete externo MediatR.
2. Trazabilidad: Todo endpoint y caso de uso implementado traza a un requisito funcional de una spec aprobada.
3. Arquitectura: Dominio hexagonal con Specification Pattern aislado sin dependencias hacia infraestructura ni API.
4. Tests: Pruebas unitarias obligatorias con xUnit para especificaciones y handlers con cobertura mínima del 85%.
5. Persistencia: Acceso a SQL Server mediante adaptadores de repositorio con auditoría canónica y soft-delete.
6. Idioma: Identificadores y código en inglés; mensajes de usuario y documentación funcional en español.
