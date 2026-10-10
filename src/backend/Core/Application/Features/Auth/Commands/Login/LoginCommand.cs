using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
