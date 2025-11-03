using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Projections;

public class BrandProjectionsTests
{
    [Fact]
    public void When_ProjectToBrandDomain_Expect_AllPropertiesAreMapped()
    {
        var entity = new BrandEntity
        {
            Id = Guid.NewGuid(),
            Name = "Name",
            Description = "Description"
        };

        var projector = BrandProjections.ToDomain.Compile();
        var domain = projector(entity);

        Assert.Multiple(
            () => entity.Id = domain.Id,
            () => entity.Name = domain.Name,
            () => entity.Description = domain.Description);
    }
}