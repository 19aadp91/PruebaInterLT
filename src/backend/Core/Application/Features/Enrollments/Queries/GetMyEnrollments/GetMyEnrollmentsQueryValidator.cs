using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Enrollments.Queries;

public class GetMyEnrollmentsQueryValidator : IValidator<GetMyEnrollmentsQuery>
{
    public Task ValidateAsync(GetMyEnrollmentsQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La consulta no puede ser nula.");

        if (request.StudentId == Guid.Empty)
            throw new DomainValidationException("El identificador del estudiante es inválido.");

        return Task.CompletedTask;
    }
}
