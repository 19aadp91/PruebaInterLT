using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Catalog.Queries;

public class GetCoursesQueryValidator : IValidator<GetCoursesQuery>
{
    public Task<Result> ValidateAsync(GetCoursesQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La consulta de materias no puede ser nula.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
