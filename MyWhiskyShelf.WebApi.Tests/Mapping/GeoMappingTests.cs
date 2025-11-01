using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Tests.Mapping;

public class GeoMappingTests
{
    [Fact]
    public void When_MappingGeoCountriesToResponseWithNestedRegions_Expect_GeoResponseMatches()
    {
        var firstCountryId = Guid.NewGuid();
        var firstRegionId = Guid.NewGuid();
        var secondRegionId = Guid.NewGuid();

        var secondCountryId = Guid.NewGuid();
        var thirdRegionId = Guid.NewGuid();

        var countries = new List<Country>
        {
            new()
            {
                Id = firstCountryId,
                Name = "First Country",
                IsActive = true,
                Regions =
                [
                    new Region
                    {
                        Id = firstRegionId,
                        CountryId = firstCountryId,
                        Name = "First Region",
                        IsActive = true
                    },
                    new Region
                    {
                        Id = secondRegionId,
                        CountryId = firstCountryId,
                        Name = "Second Region",
                        IsActive = false
                    }
                ]
            },
            new()
            {
                Id = secondCountryId,
                Name = "Second Country",
                IsActive = false,
                Regions =
                [
                    new Region
                    {
                        Id = thirdRegionId,
                        CountryId = secondCountryId,
                        Name = "Third Region",
                        IsActive = true
                    }
                ]
            }
        };

        var response = countries.ToResponse();
        Assert.NotNull(response);
        Assert.NotNull(response.Countries);

        Assert.Multiple(
            () => Assert.Equal(2, response.Countries.Count),
            () => Assert.Equal(firstCountryId, response.Countries[0].Id),
            () => Assert.Equal("First Country", response.Countries[0].Name),
            () => Assert.True(response.Countries[0].IsActive),
            () => Assert.Equal(2, response.Countries[0].Regions.Count),
            () => Assert.Equal(firstRegionId, response.Countries[0].Regions[0].Id),
            () => Assert.Equal(firstCountryId, response.Countries[0].Regions[0].CountryId),
            () => Assert.Equal("First Region", response.Countries[0].Regions[0].Name),
            () => Assert.True(response.Countries[0].Regions[0].IsActive),
            () => Assert.Equal(secondRegionId, response.Countries[0].Regions[1].Id),
            () => Assert.Equal(firstCountryId, response.Countries[0].Regions[1].CountryId),
            () => Assert.Equal("Second Region", response.Countries[0].Regions[1].Name),
            () => Assert.False(response.Countries[0].Regions[1].IsActive),
            () => Assert.Equal(secondCountryId, response.Countries[1].Id),
            () => Assert.Equal("Second Country", response.Countries[1].Name),
            () => Assert.False(response.Countries[1].IsActive),
            () => Assert.Single(response.Countries[1].Regions),
            () => Assert.Equal(thirdRegionId, response.Countries[1].Regions[0].Id),
            () => Assert.Equal(secondCountryId, response.Countries[1].Regions[0].CountryId),
            () => Assert.Equal("Third Region", response.Countries[1].Regions[0].Name),
            () => Assert.True(response.Countries[1].Regions[0].IsActive)
        );
    }
}