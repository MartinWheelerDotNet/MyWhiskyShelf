using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public static class RegionEntityTestData
{
    public static RegionEntity ActiveRegion(Guid id, Guid countryId)
    {
        return new RegionEntity
        {
            Id = id,
            CountryId = countryId,
            Name = "Active Region",
            Slug = "active-region",
            IsActive = true
        };
    }
    
    public static RegionEntity InactiveRegion(Guid id, Guid countryId)
    {
        return new RegionEntity
        {
            Id = id,
            CountryId = countryId,
            Name = "inactive Region",
            Slug = "inactive-region",
            IsActive = false
        };
    }
}