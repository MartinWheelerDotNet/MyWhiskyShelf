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
            IsActive = entity.IsActive
        };
    }

    public static RegionEntity ToEntity(this Region region, CountryEntity countryEntity)
    {
        return new RegionEntity
        {
            Id = region.Id,
            CountryId = countryEntity.Id,
            Name = region.Name.Trim(),
            IsActive = region.IsActive
        };
    }
}