using Api.Common;
using Domain.Common;
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

            _logger.LogError(actualException, "Excepción inesperada interceptada en middleware: {Message}", actualException.Message);
            await HandleExceptionAsync(context, actualException);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (errorType, message) = exception switch
        {
            UnauthorizedAccessException unauthorizedEx => (ErrorType.Unauthorized, unauthorizedEx.Message),
            _ => (ErrorType.InternalServerError, "Ocurrió un error inesperado al procesar la solicitud.")
        };

        var result = Result<object?>.Failure(message, errorType);
        context.Response.StatusCode = errorType.ToHttpStatusCode();

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(result, jsonOptions);

        return context.Response.WriteAsync(json);
    }
}
