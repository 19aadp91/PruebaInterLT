using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public class AdminCreateProfessorCommandValidator : IValidator<AdminCreateProfessorCommand>
{
    public Task<Result> ValidateAsync(AdminCreateProfessorCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La solicitud no puede ser nula.", ErrorType.Validation));

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FullName))
            errors.Add("El nombre del profesor es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
            errors.Add("El correo del profesor es inválido.");

        if (errors.Count > 0)
            return Task.FromResult(Result.Failure("Errores de validación al crear el docente.", ErrorType.Validation, errors));

        return Task.FromResult(Result.Success());
    }
}
