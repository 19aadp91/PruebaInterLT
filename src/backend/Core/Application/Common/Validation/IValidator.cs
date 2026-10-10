using Domain.Common;

namespace Application.Common.Validation;

public interface IValidator<in TRequest>
{
    Task<Result> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
