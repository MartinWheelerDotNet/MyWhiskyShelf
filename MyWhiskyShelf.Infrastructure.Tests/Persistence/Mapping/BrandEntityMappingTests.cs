using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class BrandEntityMappingTests
{
    [Fact]
    public void When_MappingBrandEntityToDomain_Expect_BrandMatches()
    {
        var countryId = Guid.NewGuid();
        var entity = new BrandEntity
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Description = "Description",
            CountryId = countryId,
            Country = new CountryEntity
            {
                Id = countryId,
                Name = "CountryName"
            }
        };

        var domain = entity.ToDomain();
        
        Assert.Multiple(
            () => Assert.Equal(entity.Id, domain.Id),
            () => Assert.Equal(entity.Name, domain.Name),
            () => Assert.Equal(entity.Description, domain.Description),
            () => Assert.Equal(entity.CountryId, domain.CountryId),
            () => Assert.Equal(entity.Country.Name, domain.CountryName));
    }
    
    [Fact]
    public void When_MappingBrandDomainToEntity_Expect_BrandEntityMatches()
    {
        var countryId = Guid.NewGuid();
        var domain = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Description = "Description",
            CountryId = countryId,
            CountryName = "CountryName"
        };
        
        var entity = domain.ToEntity();
        
        Assert.Multiple(
            () => Assert.Equal(entity.Id, domain.Id),
            () => Assert.Equal(entity.Name, domain.Name),
            () => Assert.Equal(entity.Description, domain.Description),
            () => Assert.Equal(entity.CountryId, domain.CountryId));
    }
}