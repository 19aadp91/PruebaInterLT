using Application.Common.Validation;
using Domain.Exceptions;

namespace Application.Features.Admin.Commands;

public class AdminDeleteCourseCommandValidator : IValidator<AdminDeleteCourseCommand>
{
    public Task ValidateAsync(AdminDeleteCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new DomainValidationException("La solicitud no puede ser nula.");

        if (request.CourseId == Guid.Empty)
            throw new DomainValidationException("El identificador de la materia es inválido.");

        return Task.CompletedTask;
    }
}
