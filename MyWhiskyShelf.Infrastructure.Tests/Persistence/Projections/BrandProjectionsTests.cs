using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Projections;

public class BrandProjectionsTests
{
    [Fact]
    public void When_ProjectToBrandDomain_Expect_AllPropertiesAreMapped()
    {
        var countryId = Guid.NewGuid();
        var entity = new BrandEntity
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Description = "Description",
            CountryId = Guid.NewGuid(),
            Country = new CountryEntity
            {
                Id = countryId,
                Name = "CountryName"
                
            }
        };

        var projector = BrandProjections.ToDomain.Compile();
        var domain = projector(entity);

        Assert.Multiple(
            () => Assert.Equal(entity.Id, domain.Id),
            () => Assert.Equal(entity.Name, domain.Name),
            () => Assert.Equal(entity.Description, domain.Description),
            () => Assert.Equal(entity.CountryId, domain.CountryId),
            () => Assert.Equal(entity.Country.Name, domain.CountryName));
    }
}