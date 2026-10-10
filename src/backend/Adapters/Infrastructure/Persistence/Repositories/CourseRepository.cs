using Application.Ports;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Course?> GetByIdWithProfessorAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Courses
            .Include(c => c.Professor)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Course>> GetAllWithProfessorAsync(CancellationToken ct = default)
    {
        return await _context.Courses
            .Include(c => c.Professor)
            .OrderBy(c => c.Code)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.Courses.AnyAsync(c => c.Code == code, ct);
    }

    public async Task AddAsync(Course course, CancellationToken ct = default)
    {
        await _context.Courses.AddAsync(course, ct);
    }

    public Task UpdateAsync(Course course, CancellationToken ct = default)
    {
        _context.Courses.Update(course);
        return Task.CompletedTask;
    }
}
