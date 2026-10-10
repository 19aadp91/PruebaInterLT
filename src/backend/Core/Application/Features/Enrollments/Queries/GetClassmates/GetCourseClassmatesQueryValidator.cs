using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Enrollments.Queries;

public class GetCourseClassmatesQueryValidator : IValidator<GetCourseClassmatesQuery>
{
    public Task ValidateAsync(GetCourseClassmatesQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La consulta no puede ser nula.");

        if (request.CourseId == Guid.Empty)
            throw new DomainValidationException("El identificador de la materia es inválido.");

        return Task.CompletedTask;
    }
}
