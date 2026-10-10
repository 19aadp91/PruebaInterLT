using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Enrollments.Commands;

public class DropCourseCommandValidator : IValidator<DropCourseCommand>
{
    public Task<Result> ValidateAsync(DropCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La solicitud de cancelación no puede ser nula.", ErrorType.Validation));

        var errors = new List<string>();

        if (request.StudentId == Guid.Empty)
            errors.Add("El identificador del estudiante es inválido.");

        if (request.CourseId == Guid.Empty)
            errors.Add("El identificador de la materia es inválido.");

        if (errors.Count > 0)
            return Task.FromResult(Result.Failure("Errores de validación en la solicitud de cancelación.", ErrorType.Validation, errors));

        return Task.FromResult(Result.Success());
    }
}
