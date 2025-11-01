using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class RegionEntityTestData
{
    public static RegionEntity Generic(string name, Guid countryId)
    {
        return new RegionEntity
        {
            Name = name,
            IsActive = true,
            CountryId = countryId
        };
    }
}