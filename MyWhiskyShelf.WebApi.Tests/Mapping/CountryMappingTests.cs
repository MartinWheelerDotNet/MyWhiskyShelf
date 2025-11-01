using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Tests.Mapping;

public class CountryMappingTests
{
    [Fact]
    public void When_MappingCountryToResponseWithAllFields_Expect_CountryResponseMatches()
    {
        var countryId = Guid.NewGuid();
        var firstRegionId = Guid.NewGuid();
        var secondRegionId = Guid.NewGuid();

        var domain = new Country
        {
            Id = countryId,
            Name = "Country",
            IsActive = true,
            Regions =
            [
                new Region
                {
                    Id = firstRegionId,
                    CountryId = countryId,
                    Name = "First Region",
                    IsActive = true
                },
                new Region
                {
                    Id = secondRegionId,
                    CountryId = countryId,
                    Name = "Second Region",
                    IsActive = false
                }
            ]
        };

        var response = domain.ToResponse();

        Assert.Multiple(
            () => Assert.Equal(domain.Id, response.Id),
            () => Assert.Equal(domain.Name, response.Name),
            () => Assert.Equal(domain.IsActive, response.IsActive),
            () => Assert.Equal(domain.Regions.Count, response.Regions.Count),
            () =>
            {
                var firstRegionResponse = response.Regions[0];
                var firstRegionDomain = domain.Regions[0];
                Assert.Multiple(
                    () => Assert.Equal(firstRegionDomain.Id, firstRegionResponse.Id),
                    () => Assert.Equal(firstRegionDomain.CountryId, firstRegionResponse.CountryId),
                    () => Assert.Equal(firstRegionDomain.Name, firstRegionResponse.Name),
                    () => Assert.Equal(firstRegionDomain.IsActive, firstRegionResponse.IsActive)
                );
            },
            () =>
            {
                var secondRegionResponse = response.Regions[1];
                var secondRegionDomain = domain.Regions[1];
                Assert.Multiple(
                    () => Assert.Equal(secondRegionDomain.Id, secondRegionResponse.Id),
                    () => Assert.Equal(secondRegionDomain.CountryId, secondRegionResponse.CountryId),
                    () => Assert.Equal(secondRegionDomain.Name, secondRegionResponse.Name),
                    () => Assert.Equal(secondRegionDomain.IsActive, secondRegionResponse.IsActive)
                );
            }
        );
    }

    [Fact]
    public void When_MappingCreateCountryRequestToDomainWithAllFields_Expect_CountryMatches()
    {
        var request = new CountryCreateRequest
        {
            Name = "Country",
            IsActive = true
        };

        var domain = request.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(Guid.Empty, domain.Id),
            () => Assert.Equal(request.Name, domain.Name),
            () => Assert.Equal(request.IsActive, domain.IsActive)
        );
    }

    [Fact]
    public void When_MappingUpdateCountryRequestToDomainWithAllFields_Expect_CountryMatches()
    {
        var id = Guid.NewGuid();
        var request = new UpdateCountryRequest
        {
            Id = id,
            Name = "Country",
            IsActive = false
        };

        var domain = request.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(request.Id, domain.Id),
            () => Assert.Equal(request.Name, domain.Name),
            () => Assert.Equal(request.IsActive, domain.IsActive)
        );
    }
}