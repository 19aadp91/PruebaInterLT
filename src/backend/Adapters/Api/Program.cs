using Api.Middleware;
using Application.Common.Dispatcher;
using Application.Extension;
using Infrastructure.Extension;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyección de dependencias de la Arquitectura Hexagonal
builder.Services.AddApplicationDispatcher(typeof(Application.Common.Dispatcher.NativeDispatcher).Assembly);
builder.Services.AddInfrastructureServices(builder.Configuration);

// 2. Controladores y JSON CamelCase
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// 3. Configuración de Seguridad y JWT
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "SecretKeyVeryLongAndSecureForSigningTokens1234567890!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StudentRegistrationApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StudentRegistrationApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
        };
    });

builder.Services.AddAuthorization();

// 4. Configuración de CORS para Frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. Configuración de Swagger / OpenAPI con soporte para JWT Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Registro Académico - Inter Rapidísimo",
        Version = "v1",
        Description = "API RESTful para el Sistema de Registro de Estudiantes y Selección de Materias. Soporta autenticación JWT, regla de negocio de hasta 3 materias (9 créditos) y no repetición de profesor."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT obtenido del endpoint de Login (/api/auth/login). Ejemplo: Bearer {token} o únicamente {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 6. Inicialización de la base de datos y semillas (5 profesores, 10 materias, admin)
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await seeder.InitializeAsync();
}

// 7. Pipeline HTTP
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

app.MapControllers();

app.Run();
