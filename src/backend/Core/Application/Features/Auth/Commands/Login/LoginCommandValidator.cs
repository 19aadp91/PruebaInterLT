using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Auth.Commands;

public class LoginCommandValidator : IValidator<LoginCommand>
{
    public Task ValidateAsync(LoginCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new UnauthorizedException("Credenciales de acceso inválidas.");

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new UnauthorizedException("El correo electrónico es requerido.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new UnauthorizedException("La contraseña es requerida.");

        return Task.CompletedTask;
    }
}
