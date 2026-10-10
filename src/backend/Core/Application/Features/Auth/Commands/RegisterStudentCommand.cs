using Application.Common.Dispatcher;
using Application.DTOs;

namespace Application.Features.Auth.Commands;

public record RegisterStudentCommand(
    string FullName,
    string IdentificationNumber,
    string Email,
    string Password
) : IRequest<AuthResponse>;
