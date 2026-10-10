using Domain.Entities;

namespace Application.Ports;

public interface IProfessorRepository
{
    Task<Professor?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Professor>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Professor professor, CancellationToken ct = default);
    Task UpdateAsync(Professor professor, CancellationToken ct = default);
}
