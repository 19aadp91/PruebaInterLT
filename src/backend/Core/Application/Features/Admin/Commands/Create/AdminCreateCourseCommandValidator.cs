using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public class AdminCreateCourseCommandValidator : IValidator<AdminCreateCourseCommand>
{
    public Task<Result> ValidateAsync(AdminCreateCourseCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La solicitud no puede ser nula.", ErrorType.Validation));

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Code))
            errors.Add("El código de la materia es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("El nombre de la materia es obligatorio.");

        if (request.ProfessorId == Guid.Empty)
            errors.Add("El identificador del profesor asignado es inválido.");

        if (errors.Count > 0)
            return Task.FromResult(Result.Failure("Errores de validación al crear la materia.", ErrorType.Validation, errors));

        return Task.FromResult(Result.Success());
    }
}
