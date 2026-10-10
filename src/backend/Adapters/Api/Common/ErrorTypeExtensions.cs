using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Api.Common;

public static class ErrorTypeExtensions
{
    public static int ToHttpStatusCode(this ErrorType errorType, int defaultSuccessCode = StatusCodes.Status200OK) =>
        errorType switch
        {
            ErrorType.None => defaultSuccessCode,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
}
