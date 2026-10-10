using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Catalog.Queries;

public class GetStudentProfileQueryValidator : IValidator<GetStudentProfileQuery>
{
    public Task ValidateAsync(GetStudentProfileQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La consulta de perfil no puede ser nula.");

        if (request.StudentId == Guid.Empty)
            throw new DomainValidationException("El identificador del estudiante es inválido.");

        return Task.CompletedTask;
    }
}
