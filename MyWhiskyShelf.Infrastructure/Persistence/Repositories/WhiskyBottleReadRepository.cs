using System.Diagnostics.CodeAnalysis;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

// Repository level tests are covered by integration tests, and specific functionality, such as postgres functions
// cannot be tested against sqlite / in-memory db.
[ExcludeFromCodeCoverage]
public sealed class WhiskyBottleReadRepository(MyWhiskyShelfDbContext dbContext) : IWhiskyBottleReadRepository
{
    public async Task<WhiskyBottle?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return (await dbContext.WhiskyBottles.FindAsync([id], ct))?.ToDomain();
    }
}