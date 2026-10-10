namespace Application.Common.Validation;

public interface IValidator<in TRequest>
{
    Task ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
