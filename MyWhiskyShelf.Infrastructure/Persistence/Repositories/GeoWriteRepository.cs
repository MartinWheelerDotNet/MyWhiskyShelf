using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

// Repository level tests are covered by integration tests, and specific functionality, such as postgres functions
// cannot be tested against sqlite / in-memory db.
[ExcludeFromCodeCoverage]
public sealed class GeoWriteRepository(MyWhiskyShelfDbContext dbContext) : IGeoWriteRepository
{
    public async Task<Country> AddCountryAsync(Country country, CancellationToken ct = default)
    {
        var entity = country.ToEntity();

        dbContext.Countries.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<bool> UpdateCountryAsync(Guid id, Country country, CancellationToken ct = default)
    {
        var existing = await dbContext.Countries.FindAsync([id], ct);
        if (existing is null) return false;

        var updated = country.ToEntity();
        updated.Id = id;

        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> SetCountryActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var existing = await dbContext.Countries.FindAsync([id], ct);
        if (existing is null)
            return false;

        if (existing.IsActive == isActive)
            return true;

        existing.IsActive = isActive;
        foreach (var regionEntity in dbContext.Regions.Where(x => x.CountryId == id))
            if (regionEntity.IsActive != isActive)
                regionEntity.IsActive = isActive;

        await dbContext.SaveChangesAsync(ct);
        return true;
    }


    public async Task<Region?> AddRegionAsync(Guid countryId, Region region, CancellationToken ct = default)
    {
        var parentCountry = await dbContext.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == countryId, ct);

        if (parentCountry is null) return null;

        var entity = region.ToEntity(parentCountry);
        
        dbContext.Regions.Add(entity);
        await dbContext.SaveChangesAsync(ct);

        return entity.ToDomain();
    }

    public async Task<bool> UpdateRegionAsync(Guid id, Region region, CancellationToken ct = default)
    {
        var existing = await dbContext.Regions
            .Include(r => r.Country)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
            
        if (existing is null) return false;

        var updated = region.ToEntity(existing.Country);
        updated.Id = id;

        dbContext.Entry(existing).CurrentValues.SetValues(updated);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> SetRegionActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var existing = await dbContext.Regions.FindAsync([id], ct);
        if (existing is null) return false;

        if (existing.IsActive == isActive) return true;

        existing.IsActive = isActive;
        await dbContext.SaveChangesAsync(ct);

        return true;
    }
}