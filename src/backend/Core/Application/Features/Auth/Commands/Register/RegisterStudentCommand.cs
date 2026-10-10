using Application.Common.Dispatcher;
using Application.DTOs;
using Domain.Common;

namespace Application.Features.Auth.Commands;

public record RegisterStudentCommand(
    string FullName,
    string IdentificationNumber,
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;
