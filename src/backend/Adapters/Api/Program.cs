using Api.Extension;
using Api.Middleware;
using Application.Extension;
using Infrastructure.Extension;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyección de dependencias de la Arquitectura Hexagonal y API
builder.Services.AddApplicationDispatcher();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// 2. Inicialización de la base de datos y semillas (5 profesores, 10 materias, admin)
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await seeder.InitializeAsync();
}

// 3. Pipeline HTTP
app.UseMiddleware<ProblemDetailsExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Registro Académico v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Redirección opcional desde raíz / hacia la documentación Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Health check para Application Load Balancer (ALB) y ECS Fargate
app.MapGet("/healthz", () => Results.Ok(new
{
    status = "Healthy",
    service = "StudentRegistrationApi",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapControllers();

app.Run();
