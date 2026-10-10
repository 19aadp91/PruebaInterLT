using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Auth.Commands;

public class RegisterStudentCommandValidator : IValidator<RegisterStudentCommand>
{
    public Task ValidateAsync(RegisterStudentCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La solicitud de registro no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new DomainValidationException("El nombre completo es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.IdentificationNumber))
            throw new DomainValidationException("El documento de identificación es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            throw new DomainValidationException("El correo electrónico es inválido.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new DomainValidationException("La contraseña debe tener al menos 6 caracteres.");

        return Task.CompletedTask;
    }
}
