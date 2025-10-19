using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class RegionEntityTestData
{
    public static RegionEntity Generic(string name, string slug, Guid countryId)
    {
        return new RegionEntity
        {
            Name = name,
            Slug = slug,
            IsActive = true,
            CountryId = countryId
        };
    }
}