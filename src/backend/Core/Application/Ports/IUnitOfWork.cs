namespace Application.Ports;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
