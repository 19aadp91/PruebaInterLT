using Domain.Entities;

namespace Application.Ports;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Course?> GetByIdWithProfessorAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetAllWithProfessorAsync(CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
    Task AddAsync(Course course, CancellationToken ct = default);
    Task UpdateAsync(Course course, CancellationToken ct = default);
}
