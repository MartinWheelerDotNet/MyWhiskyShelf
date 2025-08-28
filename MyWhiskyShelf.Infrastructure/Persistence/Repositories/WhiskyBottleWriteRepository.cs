using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

public sealed class WhiskyBottleWriteRepository(MyWhiskyShelfDbContext dbContext) : IWhiskyBottleWriteRepository
{
    public async Task<WhiskyBottle> AddAsync(WhiskyBottle whiskyBottle, CancellationToken ct = default)
    {
        var entity = whiskyBottle.ToEntity();
        
        dbContext.WhiskyBottles.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        return entity.ToDomain();
    }
    
    public async Task<bool> UpdateAsync(Guid id, WhiskyBottle whiskyBottle, CancellationToken ct = default)
    {
        var existing = await dbContext.WhiskyBottles.FindAsync([id], ct);
        if (existing is null) return false;
        
        var updated = whiskyBottle.ToEntity();
        updated.Id = existing.Id;
        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);
        
        return true;
    }
    
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.WhiskyBottles.FindAsync([id], ct);
        if (entity is null) return false;
        
        dbContext.WhiskyBottles.Remove(entity);
        await dbContext.SaveChangesAsync(ct);
        
        return true;
    }
}