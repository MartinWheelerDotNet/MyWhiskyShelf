using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;


namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

[ExcludeFromCodeCoverage]
public sealed class DistilleryReadRepository(MyWhiskyShelfDbContext dbContext) : IDistilleryReadRepository
{
    public async Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Distilleries
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(DistilleryProjections.ToDomain)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await dbContext.Distilleries
            .AsNoTracking()
            .AnyAsync(entity => entity.Name == name, ct);
    }

    public async Task<IReadOnlyList<Distillery>> SearchByFilter(
        DistilleryFilterOptions options,
        CancellationToken ct = default)
    {
        var query = dbContext.Distilleries.AsNoTracking();

        if (options.CountryId is { } countryId)
            query = query.Where(d => d.CountryId == countryId);

        if (options.RegionId is { } regionId)
            query = query.Where(d => d.RegionId == regionId);

        if (!string.IsNullOrWhiteSpace(options.NameSearchPattern))
        {
            var term = options.NameSearchPattern.Trim();
            var like = $"%{term}%";
            query = query.Where(d =>
                EF.Functions.ILike(d.Name, like) ||
                EF.Functions.TrigramsSimilarity(d.Name, term) >= 0.3);
        }

        query = query.OrderBy(d => d.Name);

        if (!string.IsNullOrWhiteSpace(options.AfterName))
            query = query.Where(d => string.Compare(d.Name, options.AfterName) > 0);

        return await query
            .Take(options.Amount)
            .Select(DistilleryProjections.ToDomain)
            .ToListAsync(ct);
    }
}
