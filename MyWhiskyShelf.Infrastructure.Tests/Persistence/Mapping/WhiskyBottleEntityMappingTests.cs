using MyWhiskyShelf.Infrastructure.Persistence.Mapping;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Persistence.Mapping;

public class WhiskyBottleEntityMappingTests
{
    [Fact]
    public void When_MappingWhiskyBottleEntityToDomainWithAllFields_Expect_WhiskyBottleMatches()
    {
        var whiskyBottleEntity = new WhiskyBottleEntityBuilder().WithId(Guid.NewGuid()).Build();

        var domain = whiskyBottleEntity.ToDomain();

        Assert.Multiple(
            () => Assert.Equal(whiskyBottleEntity.Id, domain.Id),
            () => Assert.Equal(whiskyBottleEntity.Name, domain.Name),
            () => Assert.Equal(whiskyBottleEntity.DistilleryName, domain.DistilleryName),
            () => Assert.Equal(whiskyBottleEntity.DistilleryId, domain.DistilleryId),
            () => Assert.Equal(whiskyBottleEntity.Status, domain.Status),
            () => Assert.Equal(whiskyBottleEntity.Bottler, domain.Bottler),
            () => Assert.Equal(whiskyBottleEntity.YearBottled, domain.YearBottled),
            () => Assert.Equal(whiskyBottleEntity.BatchNumber, domain.BatchNumber),
            () => Assert.Equal(whiskyBottleEntity.CaskNumber, domain.CaskNumber),
            () => Assert.Equal(whiskyBottleEntity.AbvPercentage, domain.AbvPercentage),
            () => Assert.Equal(whiskyBottleEntity.VolumeCl, domain.VolumeCl),
            () => Assert.Equal(whiskyBottleEntity.VolumeRemainingCl, domain.VolumeRemainingCl),
            () => Assert.Equal(whiskyBottleEntity.AddedColouring, domain.AddedColouring),
            () => Assert.Equal(whiskyBottleEntity.ChillFiltered, domain.ChillFiltered),
            () => Assert.Equal(whiskyBottleEntity.FlavourProfile, domain.FlavourProfile)
        );
    }

    [Fact]
    public void When_MappingWhiskyBottleToEntityWithAllFields_Expect_WhiskyBottleEntityMatches()
    {
        var domain = WhiskyBottleTestData.Generic;

        var entity = domain.ToEntity();

        Assert.Multiple(
            () => Assert.Equal(domain.Name, entity.Name),
            () => Assert.Equal(domain.DistilleryName, entity.DistilleryName),
            () => Assert.Equal(domain.DistilleryId, entity.DistilleryId),
            () => Assert.Equal(domain.Status, entity.Status),
            () => Assert.Equal(domain.Bottler, entity.Bottler),
            () => Assert.Equal(domain.YearBottled, entity.YearBottled),
            () => Assert.Equal(domain.BatchNumber, entity.BatchNumber),
            () => Assert.Equal(domain.CaskNumber, entity.CaskNumber),
            () => Assert.Equal(domain.AbvPercentage, entity.AbvPercentage),
            () => Assert.Equal(domain.VolumeCl, entity.VolumeCl),
            () => Assert.Equal(domain.VolumeRemainingCl, entity.VolumeRemainingCl),
            () => Assert.Equal(domain.AddedColouring, entity.AddedColouring),
            () => Assert.Equal(domain.ChillFiltered, entity.ChillFiltered),
            () => Assert.Equal(domain.FlavourProfile, entity.FlavourProfile)
        );
    }
}