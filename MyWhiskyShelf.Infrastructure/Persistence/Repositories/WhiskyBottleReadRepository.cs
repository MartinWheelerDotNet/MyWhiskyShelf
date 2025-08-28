using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

public sealed class WhiskyBottleReadRepository(MyWhiskyShelfDbContext dbContext) : IWhiskyBottleReadRepository
{
    public async Task<WhiskyBottle?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await dbContext.WhiskyBottles.FindAsync([id], ct))?.ToDomain();
}