using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Mapping;

public static class RegionEntityMapping
{
    public static Region ToDomain(this RegionEntity entity)
    {
        return new Region
        {
            Id = entity.Id,
            CountryId = entity.CountryId,
            Name = entity.Name,
            Slug = entity.Slug,
            IsActive = entity.IsActive
        };
    }

    public static RegionEntity ToEntity(this Region region, Guid countryId)
    {
        return new RegionEntity
        {
            CountryId = countryId,
            Name = region.Name.Trim(),
            Slug = region.Slug.Trim(),
            IsActive = region.IsActive
        };
    }
}