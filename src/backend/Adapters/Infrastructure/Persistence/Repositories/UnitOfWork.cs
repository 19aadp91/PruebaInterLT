using Application.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await context.SaveChangesAsync(ct);
    }
}
