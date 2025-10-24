using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class DistilleryEntityMappingTests
{
    [Fact]
    public void When_MappingDistilleryEntityToDomainWithAllFields_Expect_DistilleryMatches()
    {
        var distilleryEntity = new DistilleryEntityBuilder()
            .WithId(Guid.NewGuid())
            .AddRegion()
            .Build();

        var domain = distilleryEntity.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(distilleryEntity.Id, domain.Id),
            () => Assert.Equal(distilleryEntity.Name, domain.Name),
            () => Assert.Equal(distilleryEntity.CountryId, domain.CountryId),
            () => Assert.Equal(distilleryEntity.Country.Name, domain.CountryName),
            () => Assert.Equal(distilleryEntity.RegionId, domain.RegionId),
            () => Assert.Equal(distilleryEntity.Region!.Name, domain.RegionName),
            () => Assert.Equal(distilleryEntity.Founded, domain.Founded),
            () => Assert.Equal(distilleryEntity.Owner, domain.Owner),
            () => Assert.Equal(distilleryEntity.Type, domain.Type),
            () => Assert.Equal(distilleryEntity.FlavourVector, domain.FlavourProfile.ToVector()),
            () => Assert.Equal(distilleryEntity.Active, domain.Active));
    }
    
    [Fact]
    public void When_MappingDistilleryEntityToDomainWithoutRegion_Expect_DistilleryDoesNotContainRegionOrRegionId()
    {
        var distilleryEntity = new DistilleryEntityBuilder()
            .WithId(Guid.NewGuid())
            .Build();

        var domain = distilleryEntity.ToDomain();

        Assert.Multiple(
            () => Assert.Null(domain.RegionId),
            () => Assert.Null(distilleryEntity.Region));
    }
    

    [Fact]
    public void When_MappingDistilleryToEntityWithWhitespaceInName_Expect_NameTrimmed()
    {
        var domain = DistilleryTestData.Generic with { Name = "  Whitespace Name  " };

        var entity = domain.ToEntity();

        Assert.Equal("Whitespace Name", entity.Name);
    }
}