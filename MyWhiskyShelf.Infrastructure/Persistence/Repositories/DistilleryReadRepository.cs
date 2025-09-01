using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

// Repository level tests are covered by integration tests, and specific functionality, such as postgres functions
// cannot be tested against sqlite / in-memory db.
[ExcludeFromCodeCoverage]
public sealed class DistilleryReadRepository(MyWhiskyShelfDbContext dbContext) : IDistilleryReadRepository
{
    public async Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return (await dbContext.Distilleries.FindAsync([id], ct))?.ToDomain();
    }

    // Until the work for the migration which will allow us to use the postgres functions for case insensitivity
    // we need to silence this warning, as EF projections do not allow string comparers.
    [SuppressMessage(
        "Performance",
        "CA1862:Use the \'StringComparison\' method overloads to perform case-insensitive string comparisons")]
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await dbContext.Distilleries
            .AsNoTracking()
            .AnyAsync(e => e.Name.ToLower() == name.ToLower(), ct);
    }

    public async Task<IReadOnlyList<Distillery>> GetAllAsync(CancellationToken ct = default)
    {
        return (await dbContext.Distilleries
                .AsNoTracking()
                .OrderBy(entity => entity.Name)
                .ToListAsync(ct))
            .Select(entity => entity.ToDomain()).ToList();
    }
}