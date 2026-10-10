using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

public class AdminDeleteProfessorCommandValidator : IValidator<AdminDeleteProfessorCommand>
{
    public Task ValidateAsync(AdminDeleteProfessorCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La solicitud no puede ser nula.");

        if (request.ProfessorId == Guid.Empty)
            throw new DomainValidationException("El identificador del profesor es inválido.");

        return Task.CompletedTask;
    }
}
