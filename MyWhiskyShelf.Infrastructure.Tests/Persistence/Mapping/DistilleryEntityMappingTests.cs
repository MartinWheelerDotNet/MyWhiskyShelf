using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class DistilleryEntityMappingTests
{
    [Fact]
    public void When_MappingDistilleryEntityToDomainWithAllFields_Expect_DistilleryMatchesMatches()
    {
        var distilleryEntity = new DistilleryEntityBuilder().WithId(Guid.NewGuid()).Build();

        var domain = distilleryEntity.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(distilleryEntity.Id, domain.Id),
            () => Assert.Equal(distilleryEntity.Name, domain.Name),
            () => Assert.Equal(distilleryEntity.Location, domain.Location),
            () => Assert.Equal(distilleryEntity.Region, domain.Region),
            () => Assert.Equal(distilleryEntity.Founded, domain.Founded),
            () => Assert.Equal(distilleryEntity.Owner, domain.Owner),
            () => Assert.Equal(distilleryEntity.Type, domain.Type),
            () => Assert.Equal(distilleryEntity.FlavourProfile, domain.FlavourProfile),
            () => Assert.Equal(distilleryEntity.Active, domain.Active));
    }

    [Fact]
    public void When_MappingDistilleryToEntityWithWhitespaceInName_Expect_NameTrimmed()
    {
        var domain = DistilleryTestData.Generic with { Name = "  Whitespace Name  " };

        var entity = domain.ToEntity();

        Assert.Equal("Whitespace Name", entity.Name);
    }
}