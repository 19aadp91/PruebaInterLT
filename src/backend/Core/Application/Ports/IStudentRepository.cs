using Domain.Entities;

namespace Application.Ports;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Student?> GetByIdWithUserAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByIdentificationNumberAsync(string identificationNumber, CancellationToken ct = default);
    Task<IReadOnlyList<Student>> GetAllPublicAsync(CancellationToken ct = default);
    Task AddAsync(Student student, CancellationToken ct = default);
}
