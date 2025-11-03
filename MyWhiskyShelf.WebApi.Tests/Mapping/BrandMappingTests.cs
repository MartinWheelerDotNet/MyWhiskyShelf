using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Tests.Mapping;

public class BrandMappingTests
{
    [Fact]
    public void When_MappingBrandToResponse_Expect_BrandResponseMatches()
    {
        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Description = "Description"
        };

        var response = brand.ToResponse();
        
        Assert.Multiple(
            () => Assert.Equal(brand.Id, response.Id),
            () => Assert.Equal(brand.Name, response.Name),
            () => Assert.Equal(brand.Description, response.Description));
    }
}