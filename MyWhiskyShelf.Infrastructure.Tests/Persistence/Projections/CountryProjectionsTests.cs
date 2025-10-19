using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Projections;

public class CountryProjectionsTests
{
    [Fact]
    public void When_ProjectToCountryDomain_Expect_AllPropertiesAreMapped()
    {
        var countryId = Guid.NewGuid();
        var inactiveRegionId = Guid.NewGuid();
        var activeRegionId = Guid.NewGuid();
        List<Region> expectedRegions =
        [
            RegionTestData.ActiveRegion(countryId) with { Id = activeRegionId },
            RegionTestData.InactiveRegion(countryId) with { Id = inactiveRegionId }
        ];
        
        var countryEntity = new CountryEntity
        {
            Id = countryId,
            Name = "Country",
            Slug = "country",
            IsActive = false,
            Regions = [
                RegionEntityTestData.ActiveRegion(activeRegionId, countryId),
                RegionEntityTestData.InactiveRegion(inactiveRegionId, countryId)
            ]
        };

        var projector = CountryProjections.ToCountryDomain.Compile();
        var country = projector(countryEntity);

        Assert.Multiple(
            () => Assert.Equal(countryEntity.Id, country.Id),
            () => Assert.Equal(countryEntity.Name, country.Name),
            () => Assert.Equal(countryEntity.Slug, country.Slug),
            () => Assert.Equal(countryEntity.IsActive, country.IsActive),
            () => Assert.Equal(expectedRegions.OrderBy(er => er.Name).ThenBy(er => er.Id), country.Regions));
    }
}