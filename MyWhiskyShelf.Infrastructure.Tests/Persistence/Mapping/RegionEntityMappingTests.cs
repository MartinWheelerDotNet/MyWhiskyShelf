using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class RegionEntityMappingTests
{
    [Fact]
    public void When_MappingRegionEntityToDomain_Expect_RegionMatches()
    {
        var id = Guid.NewGuid();
        var countryId = Guid.NewGuid();
        var regionEntity = new RegionEntity
        {
            Id = id,
            Name = "Region",
            Slug = "region",
            IsActive = true,
            CountryId = countryId
        };

        var region = regionEntity.ToDomain();
        
        Assert.Multiple(
            () => Assert.Equal(regionEntity.CountryId, region.CountryId),
            () => Assert.Equal("Region", region.Name),
            () => Assert.Equal("region", region.Slug),
            () => Assert.True(region.IsActive));
    }

    [Fact]
    public void When_MappingRegionToEntity_Expect_RegionEntityMatches()
    {
        var countryId = Guid.NewGuid();
        var region = RegionTestData.ActiveRegion(countryId);
        
        var regionEntity = region.ToEntity(countryId);
        
        Assert.Multiple(
            () => Assert.Equal(region.CountryId, regionEntity.CountryId),
            () => Assert.Equal("Active Region", regionEntity.Name),
            () => Assert.Equal("active-region", regionEntity.Slug),
            () => Assert.True(regionEntity.IsActive));
    }
}