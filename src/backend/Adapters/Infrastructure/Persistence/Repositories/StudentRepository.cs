using Application.Ports;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Students.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<Student?> GetByIdWithUserAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<bool> ExistsByIdentificationNumberAsync(string identificationNumber, CancellationToken ct = default)
    {
        return await _context.Students.AnyAsync(s => s.IdentificationNumber == identificationNumber, ct);
    }

    public async Task<IReadOnlyList<Student>> GetAllPublicAsync(CancellationToken ct = default)
    {
        return await _context.Students
            .Include(s => s.User)
            .OrderBy(s => s.User!.FullName)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Student student, CancellationToken ct = default)
    {
        await _context.Students.AddAsync(student, ct);
    }
}
