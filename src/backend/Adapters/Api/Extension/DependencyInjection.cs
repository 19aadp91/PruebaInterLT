using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Api.Extension;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Controladores y formateo JSON
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

        // 2. Configuración de Seguridad y JWT
        services.AddJwtAuthentication(configuration);

        // 3. Política de CORS
        services.AddCorsPolicy();

        // 4. Configuración de Swagger / OpenAPI
        services.AddSwaggerDocumentation();

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecretKey = configuration["Jwt:SecretKey"] ?? "SecretKeyVeryLongAndSecureForSigningTokens1234567890!";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "StudentRegistrationApi";
        var jwtAudience = configuration["Jwt:Audience"] ?? "StudentRegistrationApp";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        services.AddAuthorization();

        return services;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }

    private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
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

        return services;
    }
}
