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
            () => Assert.Equal(entity.Location, model.Location),
            () => Assert.Equal(entity.Region, model.Region),
            () => Assert.Equal(entity.Founded, model.Founded),
            () => Assert.Equal(entity.Owner, model.Owner),
            () => Assert.Equal(entity.Type, model.Type),
            () => Assert.Equal(entity.FlavourProfile, model.FlavourProfile),
            () => Assert.Equal(entity.Active, model.Active)
        );
    }
    
    [Fact]
    public void WhenProjectToDistilleryNameDomain_Expect_IdAndNameAreMapped()
    {
        var id = Guid.NewGuid();
        var entity = new TestData.DistilleryEntityBuilder().WithId(id).Build();

        var projector = DistilleryProjections.ToDistilleryNameDomain.Compile();
        var model = projector(entity);

        Assert.Multiple(
            () => Assert.NotNull(model),
            () => Assert.Equal(entity.Id, model.Id),
            () => Assert.Equal(entity.Name, model.Name)
        );
    }
}