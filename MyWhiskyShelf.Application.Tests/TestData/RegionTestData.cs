using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Tests.TestData;

public static class RegionTestData
{
    public static Region ActiveRegion(Guid countryId, Guid id)
    {
        return new Region
        {
            Id = id,
            CountryId = countryId,
            Name = "Active Region",
            IsActive = true
        };
    }

    public static Region InactiveRegion(Guid countryId, Guid id)
    {
        return new Region
        {
            Id = id,
            CountryId = countryId,
            Name = "Inactive Region",
            IsActive = false
        };
    }
}