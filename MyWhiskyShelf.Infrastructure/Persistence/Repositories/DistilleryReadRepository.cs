using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

public sealed class DistilleryReadRepository(MyWhiskyShelfDbContext dbContext) : IDistilleryReadRepository
{
    public async Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await dbContext.Distilleries.FindAsync([id], ct))?.ToDomain();
        

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) 
        => await dbContext.Distilleries
            .AsNoTracking( )
            .AnyAsync(e => e.Name.ToLower() == name.ToLower(), ct);
    
    public async Task<IReadOnlyList<Distillery>> GetAllAsync(CancellationToken ct = default)
        => (await dbContext.Distilleries
                .AsNoTracking()
                .OrderBy(entity => entity.Name)
                .ToListAsync(ct))
            .Select(entity => entity.ToDomain()).ToList();
}