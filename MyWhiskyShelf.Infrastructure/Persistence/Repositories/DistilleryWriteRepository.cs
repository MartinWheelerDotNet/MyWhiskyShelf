using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

public sealed class DistilleryWriteRepository(MyWhiskyShelfDbContext dbContext) : IDistilleryWriteRepository
{
    public async Task<Distillery?> AddAsync(Distillery distillery, CancellationToken ct = default)
    {
        var entity = distillery.ToEntity();
        
        dbContext.Distilleries.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        return entity.ToDomain();
    }
    
    public async Task<bool> UpdateAsync(Guid id, Distillery distillery, CancellationToken ct = default)
    {
        var existingEntity = await dbContext.Distilleries.FindAsync([id], ct);
        if (existingEntity is null) return false;

        var updated = distillery.ToEntity();
        updated.Id = id;
        dbContext.Entry(existingEntity).CurrentValues.SetValues(updated);
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