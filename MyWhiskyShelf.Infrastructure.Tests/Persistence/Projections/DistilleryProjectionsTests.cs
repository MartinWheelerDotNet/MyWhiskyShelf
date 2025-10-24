using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Projections;

public class DistilleryProjectionsTests
{
    [Fact]
    public void When_ProjectToDistilleryDomainWithAllValueSet_Expect_AllPropertiesAreMapped()
    {
        var id = Guid.NewGuid();
        var entity = new DistilleryEntityBuilder()
            .WithId(id)
            .AddRegion()
            .Build();

        var projector = DistilleryProjections.ToDistilleryDomain.Compile();
        var model = projector(entity);

        Assert.Multiple(
            () => Assert.NotNull(model),
            () => Assert.Equal(entity.Id, model.Id),
            () => Assert.Equal(entity.Name, model.Name),
            () => Assert.Equal(entity.CountryId, model.CountryId),
            () => Assert.Equal(entity.Country.Name, model.CountryName),
            () => Assert.Equal(entity.RegionId, model.RegionId),
            () => Assert.Equal(entity.Region?.Name, model.RegionName),
            () => Assert.Equal(entity.Founded, model.Founded),
            () => Assert.Equal(entity.Owner, model.Owner),
            () => Assert.Equal(entity.Type, model.Type),
            () => Assert.Equal(entity.FlavourVector, model.FlavourProfile.ToVector()),
            () => Assert.Equal(entity.Active, model.Active)
        );
    }
    
    [Fact]
    public void When_ProjectToDistilleryDomainWithoutRegion_Expect_RegionNameAndRegionIdAreNotPopulated()
    {
        var id = Guid.NewGuid();
        var entity = new DistilleryEntityBuilder()
            .WithId(id)
            .Build();

        var projector = DistilleryProjections.ToDistilleryDomain.Compile();
        var model = projector(entity);

        Assert.Multiple(
            () => Assert.Null(model.RegionId),
            () => Assert.Null(model.RegionName)
        );
    }
}