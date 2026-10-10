using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Auth.Commands;

public class LoginCommandValidator : IValidator<LoginCommand>
{
    public Task<Result> ValidateAsync(LoginCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("Credenciales de acceso inválidas.", ErrorType.Unauthorized));

        if (string.IsNullOrWhiteSpace(request.Email))
            return Task.FromResult(Result.Failure("El correo electrónico es requerido.", ErrorType.Validation));

        if (string.IsNullOrWhiteSpace(request.Password))
            return Task.FromResult(Result.Failure("La contraseña es requerida.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
