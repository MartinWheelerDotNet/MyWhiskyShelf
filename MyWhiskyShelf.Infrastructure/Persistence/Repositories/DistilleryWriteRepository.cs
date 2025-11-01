using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

// Repository level tests are covered by integration tests, and specific functionality, such as postgres functions
// cannot be tested against sqlite / in-memory db.
[ExcludeFromCodeCoverage]
public sealed class DistilleryWriteRepository(MyWhiskyShelfDbContext dbContext) : IDistilleryWriteRepository
{
    public async Task<Distillery> AddAsync(Distillery distillery, CancellationToken ct = default)
    {
        var entity = distillery.ToEntity();
       
        dbContext.Distilleries.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        var mappedEntity = await dbContext.Distilleries
            .AsNoTracking()
            .Where(e => e.Id == entity.Id)
            .Select(DistilleryProjections.ToDomain)
            .SingleAsync(ct);
            
        return mappedEntity;
    }

    public async Task<bool> UpdateAsync(Guid id, Distillery distillery, CancellationToken ct = default)
    {
        var existing = await dbContext.Distilleries.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (existing is null) return false;

        var updated = distillery.ToEntity();
        updated.Id = id;

        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.Distilleries.FindAsync([id], ct);
        if (entity is null) return false;

        dbContext.Distilleries.Remove(entity);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
