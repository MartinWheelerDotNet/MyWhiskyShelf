using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class RegionEntityMappingTests
{
    [Fact]
    public void When_MappingRegionEntityToDomain_Expect_RegionMatches()
    {
        var id = Guid.NewGuid();
        var countryId = Guid.NewGuid();
        var countryEntity = new CountryEntity { Id = countryId };
        
        var regionEntity = new RegionEntity
        {
            Id = id,
            Name = "Region",
            IsActive = true,
            Country = countryEntity
        };

        var region = regionEntity.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(regionEntity.CountryId, region.CountryId),
            () => Assert.Equal("Region", region.Name),
            () => Assert.True(region.IsActive));
    }

    [Fact]
    public void When_MappingRegionToEntity_Expect_RegionEntityMatches()
    {
        var countryId = Guid.NewGuid();
        var countryEntity = new CountryEntity { Id = countryId };
        
        var region = new Region
        {
            CountryId = countryId,
            Name = "Region",
            IsActive = true
        };

        var regionEntity = region.ToEntity(countryEntity);
    
        Assert.Multiple(
            () => Assert.Equal(regionEntity.CountryId, countryId),
            () => Assert.Equal("Region", regionEntity.Name),
            () => Assert.True(regionEntity.IsActive));
    }
}