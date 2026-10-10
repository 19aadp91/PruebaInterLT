using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

public class AdminCreateProfessorCommandValidator : IValidator<AdminCreateProfessorCommand>
{
    public Task ValidateAsync(AdminCreateProfessorCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La solicitud no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new DomainValidationException("El nombre del profesor es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            throw new DomainValidationException("El correo del profesor es inválido.");

        return Task.CompletedTask;
    }
}
