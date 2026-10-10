using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Catalog.Queries;

public class GetCoursesQueryValidator : IValidator<GetCoursesQuery>
{
    public Task ValidateAsync(GetCoursesQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La consulta de materias no puede ser nula.");

        return Task.CompletedTask;
    }
}
