using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public static class RegionTestData
{
    public static Region ActiveRegion(Guid countryId)
    {
        return new Region
        {
            CountryId = countryId,
            Name = "Active Region",
            IsActive = true
        };
    }

    public static Region InactiveRegion(Guid countryId)
    {
        return new Region
        {
            CountryId = countryId,
            Name = "inactive Region",
            IsActive = false
        };
    }
}