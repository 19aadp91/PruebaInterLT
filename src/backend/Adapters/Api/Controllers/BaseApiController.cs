using Api.Common;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ToActionResult<T>(Result<T> result, int successStatusCode = StatusCodes.Status200OK)
    {
        var statusCode = result.IsSuccess ? successStatusCode : result.ErrorType.ToHttpStatusCode();
        return StatusCode(statusCode, result);
    }

    protected IActionResult ToActionResult(Result result, int successStatusCode = StatusCodes.Status200OK)
    {
        var statusCode = result.IsSuccess ? successStatusCode : result.ErrorType.ToHttpStatusCode();
        return StatusCode(statusCode, result);
    }
}
