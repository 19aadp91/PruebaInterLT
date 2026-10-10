namespace Domain.Common;

public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T candidate);
    string ErrorMessage { get; }
}
