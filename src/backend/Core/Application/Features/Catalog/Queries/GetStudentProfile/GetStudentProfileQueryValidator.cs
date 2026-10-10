using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Catalog.Queries;

public class GetStudentProfileQueryValidator : IValidator<GetStudentProfileQuery>
{
    public Task<Result> ValidateAsync(GetStudentProfileQuery request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La consulta de perfil no puede ser nula.", ErrorType.Validation));

        if (request.StudentId == Guid.Empty)
            return Task.FromResult(Result.Failure("El identificador del estudiante es inválido.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
