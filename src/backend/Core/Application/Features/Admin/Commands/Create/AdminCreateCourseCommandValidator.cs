using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

public class AdminCreateCourseCommandValidator : IValidator<AdminCreateCourseCommand>
{
    public Task ValidateAsync(AdminCreateCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La solicitud no puede ser nula.");

        if (string.IsNullOrWhiteSpace(request.Code))
            throw new DomainValidationException("El código de la materia es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new DomainValidationException("El nombre de la materia es obligatorio.");

        if (request.ProfessorId == Guid.Empty)
            throw new DomainValidationException("El identificador del profesor asignado es inválido.");

        return Task.CompletedTask;
    }
}
