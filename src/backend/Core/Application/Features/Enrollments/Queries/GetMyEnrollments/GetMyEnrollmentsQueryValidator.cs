using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Enrollments.Queries;

public class GetMyEnrollmentsQueryValidator : IValidator<GetMyEnrollmentsQuery>
{
    public Task<Result> ValidateAsync(GetMyEnrollmentsQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La consulta no puede ser nula.", ErrorType.Validation));

        if (request.StudentId == Guid.Empty)
            return Task.FromResult(Result.Failure("El identificador del estudiante es inválido.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
