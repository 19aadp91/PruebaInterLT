using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Auth.Commands;

public class RegisterStudentCommandValidator : IValidator<RegisterStudentCommand>
{
    public Task<Result> ValidateAsync(RegisterStudentCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La solicitud de registro no puede ser nula.", ErrorType.Validation));

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FullName))
            errors.Add("El nombre completo es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
            errors.Add("El documento de identificación es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            errors.Add("El correo electrónico es inválido.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            errors.Add("La contraseña debe tener al menos 6 caracteres.");

        if (errors.Count > 0)
            return Task.FromResult(Result.Failure("Errores de validación en los datos de registro.", ErrorType.Validation, errors));

        return Task.FromResult(Result.Success());
    }
}
