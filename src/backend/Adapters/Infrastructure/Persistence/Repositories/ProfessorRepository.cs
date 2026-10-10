using Application.Ports;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly ApplicationDbContext _context;

    public ProfessorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Professor?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Professors.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Professor>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Professors
            .OrderBy(p => p.FullName)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Professors.AnyAsync(p => p.Email == email, ct);
    }

    public async Task AddAsync(Professor professor, CancellationToken ct = default)
    {
        await _context.Professors.AddAsync(professor, ct);
    }

    public Task UpdateAsync(Professor professor, CancellationToken ct = default)
    {
        _context.Professors.Update(professor);
        return Task.CompletedTask;
    }
}
