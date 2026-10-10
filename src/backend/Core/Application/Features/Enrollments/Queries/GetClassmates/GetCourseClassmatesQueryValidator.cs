using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Enrollments.Queries;

public class GetCourseClassmatesQueryValidator : IValidator<GetCourseClassmatesQuery>
{
    public Task<Result> ValidateAsync(GetCourseClassmatesQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La consulta no puede ser nula.", ErrorType.Validation));

        if (request.CourseId == Guid.Empty)
            return Task.FromResult(Result.Failure("El identificador de la materia es inválido.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
