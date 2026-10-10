using Domain.Entities;

namespace Application.Ports;

public interface IEnrollmentRepository
{
    Task<IReadOnlyList<Enrollment>> GetActiveByStudentIdAsync(Guid studentId, CancellationToken ct = default);
    Task<Enrollment?> GetActiveByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Student>> GetClassmatesByCourseIdAsync(Guid courseId, CancellationToken ct = default);
    Task<bool> HasActiveEnrollmentsByCourseIdAsync(Guid courseId, CancellationToken ct = default);
    Task<bool> HasActiveEnrollmentsByProfessorIdAsync(Guid professorId, CancellationToken ct = default);
    Task AddAsync(Enrollment enrollment, CancellationToken ct = default);
    Task RemoveAsync(Enrollment enrollment, CancellationToken ct = default);
}
