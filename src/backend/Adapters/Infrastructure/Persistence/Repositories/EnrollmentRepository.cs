using Application.Ports;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Enrollment>> GetActiveByStudentIdAsync(Guid studentId, CancellationToken ct = default)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c!.Professor)
            .Where(e => e.StudentId == studentId)
            .OrderBy(e => e.EnrolledAtUtc)
            .ToListAsync(ct);
    }

    public async Task<Enrollment?> GetActiveByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);
    }

    public async Task<IReadOnlyList<Student>> GetClassmatesByCourseIdAsync(Guid courseId, CancellationToken ct = default)
    {
        var studentIds = await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToListAsync(ct);

        return await _context.Students
            .Include(s => s.User)
            .Where(s => studentIds.Contains(s.Id))
            .OrderBy(s => s.User!.FullName)
            .ToListAsync(ct);
    }

    public async Task<bool> HasActiveEnrollmentsByCourseIdAsync(Guid courseId, CancellationToken ct = default)
    {
        return await _context.Enrollments.AnyAsync(e => e.CourseId == courseId, ct);
    }

    public async Task<bool> HasActiveEnrollmentsByProfessorIdAsync(Guid professorId, CancellationToken ct = default)
    {
        return await _context.Enrollments
            .Include(e => e.Course)
            .AnyAsync(e => e.Course != null && e.Course.ProfessorId == professorId, ct);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        await _context.Enrollments.AddAsync(enrollment, ct);
    }

    public Task RemoveAsync(Enrollment enrollment, CancellationToken ct = default)
    {
        _context.Enrollments.Remove(enrollment);
        return Task.CompletedTask;
    }
}
