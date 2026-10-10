using Application.Common.Dispatcher;
using Application.DTOs;
using Application.Features.Auth.Commands;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v1/auth")]
public class AuthController : BaseApiController
{
    private readonly IDispatcher _dispatcher;

    public AuthController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterStudentCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync(command, ct);
        return ToActionResult(response, StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var response = await _dispatcher.SendAsync(command, ct);
        return ToActionResult(response, StatusCodes.Status200OK);
    }
}
