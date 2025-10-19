using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class CountryEntityMappingTests
{
    [Fact]
    public void When_MappingCountryEntityToDomainWithRegions_Expect_CountryMatchesAndRegionsOrdered()
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
            IsActive = true,
            Regions =
            [
                RegionEntityTestData.ActiveRegion(activeRegionId, countryId),
                RegionEntityTestData.InactiveRegion(inactiveRegionId, countryId)
            ]
        };

        var domain = countryEntity.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(countryEntity.Id, domain.Id),
            () => Assert.Equal(countryEntity.Name, domain.Name),
            () => Assert.Equal(countryEntity.Slug, domain.Slug),
            () => Assert.Equal(countryEntity.IsActive, domain.IsActive),
            () => Assert.Equal(expectedRegions, domain.Regions));
    }

    [Fact]
    public void When_MappingCountryToEntityWithWhitespace_Expect_NameAndSlugTrimmed()
    {
        var country = new Country
        {
            Id = Guid.NewGuid(),
            Name = "  Country   ",
            Slug = "country ",
            IsActive = true,
            Regions = []
        };

        var countryEntity = country.ToEntity();

        Assert.Multiple(
            () => Assert.Equal("Country", countryEntity.Name),
            () => Assert.Equal("country", countryEntity.Slug),
            () => Assert.True(countryEntity.IsActive)
        );
    }
}