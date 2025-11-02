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
public sealed class GeoReadRepository(MyWhiskyShelfDbContext dbContext) : IGeoReadRepository
{
    public async Task<IReadOnlyList<Country>> GetAllGeoInformationAsync(CancellationToken ct = default)
    {
        return await dbContext.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .Select(CountryProjections.ToCountryDomain)
            .ToListAsync(ct);
    }

    public async Task<Country?> GetCountryByIdAsync(Guid id, CancellationToken ct = default)
    {
        var countryEntity = await dbContext.Countries.FindAsync([id], ct);
        return countryEntity?.ToDomain();
    }

    public async Task<bool> CountryExistsByNameAsync(string name, CancellationToken ct = default)
    {
        return await dbContext.Countries
            .AsNoTracking()
            .AnyAsync(c => c.Name == name, ct);
    }

    public async Task<bool> CountryExistsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Countries
            .AsNoTracking()
            .AnyAsync(c => c.Id == id, ct);
    }

    public async Task<Region?> GetRegionByIdAsync(Guid id, CancellationToken ct = default)
    {
        var regionEntity = await dbContext.Regions.FindAsync([id], ct);
        return regionEntity?.ToDomain();
    }


    public async Task<bool> RegionExistsByNameAndCountryIdAsync(
        string name,
        Guid countryId,
        CancellationToken ct = default)
    {
        return await dbContext.Regions
            .AsNoTracking()
            .AnyAsync(c => c.Name == name && c.CountryId == countryId, ct);
    }
}