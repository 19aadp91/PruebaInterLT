using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Catalog.Queries;

public class GetPublicStudentsQueryValidator : IValidator<GetPublicStudentsQuery>
{
    public Task<Result> ValidateAsync(GetPublicStudentsQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La consulta de estudiantes no puede ser nula.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
