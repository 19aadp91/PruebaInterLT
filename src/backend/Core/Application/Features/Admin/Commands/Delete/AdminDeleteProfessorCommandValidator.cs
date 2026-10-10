using Application.Common.Validation;
using Domain.Common;

namespace Application.Features.Admin.Commands;

public class AdminDeleteProfessorCommandValidator : IValidator<AdminDeleteProfessorCommand>
{
    public Task<Result> ValidateAsync(AdminDeleteProfessorCommand request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            return Task.FromResult(Result.Failure("La solicitud no puede ser nula.", ErrorType.Validation));

        if (request.ProfessorId == Guid.Empty)
            return Task.FromResult(Result.Failure("El identificador del profesor es inválido.", ErrorType.Validation));

        return Task.FromResult(Result.Success());
    }
}
