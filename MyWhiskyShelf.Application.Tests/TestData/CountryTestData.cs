using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Tests.TestData;

public static class CountryTestData
{
    public static Country Generic(Guid? countryId = null)
    {
        countryId ??= Guid.NewGuid();
        return new Country
        {
            Id = countryId.Value,
            Name = "Generic Country",
            Slug = "generic-country",
            IsActive = true,
            Regions =
            [
                RegionTestData.ActiveRegion(countryId.Value, Guid.NewGuid()),
                RegionTestData.InactiveRegion(countryId.Value, Guid.NewGuid())
            ]
        };
    }
}