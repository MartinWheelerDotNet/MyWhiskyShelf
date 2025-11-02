using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Tests.Mapping;

public class RegionMappingTests
{
    [Fact]
    public void When_MappingRegionToResponseWithAllFields_Expect_RegionResponseMatches()
    {
        var id = Guid.NewGuid();
        var countryId = Guid.NewGuid();

        var domain = new Region
        {
            Id = id,
            CountryId = countryId,
            Name = "Region",
            IsActive = true
        };
        var response = domain.ToResponse();

        Assert.Multiple(
            () => Assert.Equal(domain.Id, response.Id),
            () => Assert.Equal(domain.CountryId, response.CountryId),
            () => Assert.Equal(domain.Name, response.Name),
            () => Assert.Equal(domain.IsActive, response.IsActive)
        );
    }

    [Fact]
    public void When_MappingCreateRegionRequestToDomain_Expect_SlugGeneratedFromName()
    {
        var id = Guid.NewGuid();
        var countryId = Guid.NewGuid();
        var request = new RegionCreateRequest
        {
            Id = id,
            CountryId = countryId,
            Name = "First Region",
            IsActive = false
        };

        var domain = request.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(request.Id, domain.Id),
            () => Assert.Equal(request.CountryId, domain.CountryId),
            () => Assert.Equal(request.Name, domain.Name),
            () => Assert.Equal(request.IsActive, domain.IsActive)
        );
    }

    [Fact]
    public void When_MappingUpdateRegionRequestToDomain_Expect_SlugGeneratedFromName()
    {
        // Arrange
        var id = Guid.NewGuid();
        var countryId = Guid.NewGuid();
        var request = new UpdateRegionRequest
        {
            Id = id,
            CountryId = countryId,
            Name = "Region",
            IsActive = true
        };

        var domain = request.ToDomain();

        // Assert
        Assert.Multiple(
            () => Assert.Equal(request.Id, domain.Id),
            () => Assert.Equal(request.CountryId, domain.CountryId),
            () => Assert.Equal(request.Name, domain.Name),
            () => Assert.Equal(request.IsActive, domain.IsActive)
        );
    }
}