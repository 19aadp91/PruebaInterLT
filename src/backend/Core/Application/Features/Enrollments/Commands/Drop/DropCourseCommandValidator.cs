using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Enrollments.Commands;

public class DropCourseCommandValidator : IValidator<DropCourseCommand>
{
    public Task ValidateAsync(DropCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La solicitud de cancelación no puede ser nula.");

        if (request.StudentId == Guid.Empty)
            throw new DomainValidationException("El identificador del estudiante es inválido.");

        if (request.CourseId == Guid.Empty)
            throw new DomainValidationException("El identificador de la materia es inválido.");

        return Task.CompletedTask;
    }
}
