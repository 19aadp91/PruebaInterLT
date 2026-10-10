using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Api.Middleware;

public class ProblemDetailsExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProblemDetailsExceptionMiddleware> _logger;

    public ProblemDetailsExceptionMiddleware(RequestDelegate next, ILogger<ProblemDetailsExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var actualException = ex;
            while (actualException is System.Reflection.TargetInvocationException tie && tie.InnerException != null)
            {
                actualException = tie.InnerException;
            }
            if (actualException is AggregateException agg && agg.InnerExceptions.Count == 1)
            {
                actualException = agg.InnerExceptions[0];
            }

            _logger.LogError(actualException, "Excepción interceptada en middleware: {Message}", actualException.Message);
            await HandleExceptionAsync(context, actualException);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case DomainValidationException domainEx:
                problemDetails.Title = "Violación de Regla de Negocio";
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Detail = domainEx.Message;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7807#section-3.1";
                break;

            case ConflictException conflictEx:
                problemDetails.Title = "Conflicto de Estado";
                problemDetails.Status = (int)HttpStatusCode.Conflict;
                problemDetails.Detail = conflictEx.Message;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7807#section-3.1";
                break;

            case NotFoundException notFoundEx:
                problemDetails.Title = "Recurso No Encontrado";
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Detail = notFoundEx.Message;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7807#section-3.1";
                break;

            case UnauthorizedException unauthorizedEx:
                problemDetails.Title = "Acceso No Autorizado";
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Detail = unauthorizedEx.Message;
                problemDetails.Type = "https://tools.ietf.org/html/rfc7807#section-3.1";
                break;

            default:
                problemDetails.Title = "Error Interno del Servidor";
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Detail = "Ocurrió un error inesperado al procesar la solicitud.";
                problemDetails.Type = "https://tools.ietf.org/html/rfc7807#section-3.1";
                break;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);

        return context.Response.WriteAsync(json);
    }
}
