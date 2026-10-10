using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public class AdminDeleteCourseCommandValidator : IValidator<AdminDeleteCourseCommand>
{
    public Task<Result> ValidateAsync(AdminDeleteCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La solicitud no puede ser nula.", ErrorType.Validation));

        if (request.CourseId == Guid.Empty)
            return Task.FromResult(Result.Failure("El identificador de la materia es inválido.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
