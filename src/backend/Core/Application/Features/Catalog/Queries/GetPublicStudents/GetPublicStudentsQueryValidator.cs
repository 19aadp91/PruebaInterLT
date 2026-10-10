using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Catalog.Queries;

public class GetPublicStudentsQueryValidator : IValidator<GetPublicStudentsQuery>
{
    public Task ValidateAsync(GetPublicStudentsQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La consulta de estudiantes no puede ser nula.");

        return Task.CompletedTask;
    }
}
