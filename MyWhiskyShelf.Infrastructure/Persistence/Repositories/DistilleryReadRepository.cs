using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

// Repository level tests are covered by integration tests, and specific functionality, such as postgres functions
// cannot be tested against sqlite / in-memory db.
[ExcludeFromCodeCoverage]
public sealed class DistilleryReadRepository(MyWhiskyShelfDbContext dbContext) : IDistilleryReadRepository
{
    public async Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Distilleries
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(DistilleryProjections.ToDistilleryDomain)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await dbContext.Distilleries
            .AsNoTracking()
            .AnyAsync(entity => entity.Name == name, ct);
    }
    
    public async Task<IReadOnlyList<DistilleryName>> SearchByNameAsync(string pattern, CancellationToken ct = default)
    {
        return await dbContext.Distilleries
            .AsNoTracking()
            .Where(entity => EF.Functions.ILike(entity.Name, $"%{pattern}%"))
            .Select(DistilleryProjections.ToDistilleryNameDomain)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Distillery>> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.Distilleries
                .AsNoTracking()
                .OrderBy(entity => entity.Name)
                .ThenBy(entity => entity.Id)
                .Select(DistilleryProjections.ToDistilleryDomain)
                .ToListAsync(ct);
        
    }
}