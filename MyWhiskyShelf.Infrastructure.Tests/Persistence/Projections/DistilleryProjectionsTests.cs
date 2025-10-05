using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Projections;

public class DistilleryProjectionsTests
{
    [Fact]
    public void When_ProjectToDistilleryDomain_Expect_AllPropertiesAreMapped()
    {
        var id = Guid.NewGuid();
        var entity = new TestData.DistilleryEntityBuilder().WithId(id).Build();

        var projector = DistilleryProjections.ToDistilleryDomain.Compile();
        var model = projector(entity);

        Assert.Multiple(
            () => Assert.NotNull(model),
            () => Assert.Equal(entity.Id, model.Id),
            () => Assert.Equal(entity.Name, model.Name),
            () => Assert.Equal(entity.Country, model.Country),
            () => Assert.Equal(entity.Region, model.Region),
            () => Assert.Equal(entity.Founded, model.Founded),
            () => Assert.Equal(entity.Owner, model.Owner),
            () => Assert.Equal(entity.Type, model.Type),
            () => Assert.Equal(entity.FlavourVector, model.FlavourProfile.ToVector()),
            () => Assert.Equal(entity.Active, model.Active)
        );
    }
}