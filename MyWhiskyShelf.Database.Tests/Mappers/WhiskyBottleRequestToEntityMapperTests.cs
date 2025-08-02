using Moq;
using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Mappers;

public class WhiskyBottleRequestToEntityMapperTests
{
    private readonly Mock<IDistilleryNameCacheService> _mockDistilleryNameCacheService = new();

    [Fact]
    public void When_MapToEntityWithAllValuesPopulated_Expect_EntityWithAllValuesPopulated()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var whiskyBottleEntity = whiskyBottleMapper.Map(WhiskyBottleRequestTestData.AllValuesPopulated);

        Assert.Equal(WhiskyBottleEntityTestData.AllValuesPopulated with { Id = Guid.Empty }, whiskyBottleEntity);
    }

    [Fact]
    public void When_MapToEntityWithDateBottledButNotYearBottled_Expect_EntityWithYearBottledSetFromDate()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottleEntity = whiskyBottleMapper.Map(
            WhiskyBottleRequestTestData.AllValuesPopulated with { YearBottled = null });

        Assert.Equal(WhiskyBottleEntityTestData.AllValuesPopulated.DateBottled?.Year, whiskyBottleEntity.YearBottled);
    }

    [Fact]
    public void When_MapToEntityWithoutDateBottledOrYearBottled_Expect_EntityWithoutYearBottledSet()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottleEntity = whiskyBottleMapper.Map(
            WhiskyBottleRequestTestData.AllValuesPopulated with { DateBottled = null, YearBottled = null });

        Assert.Null(whiskyBottleEntity.YearBottled);
    }

    [Fact]
    public void When_MapToEntityWithDistilleryNameInCache_Expect_EntityWithDistilleryIdSet()
    {
        var distilleryNameDetails = new DistilleryNameDetails("A DistilleryRequest Name", Guid.AllBitsSet);
        _mockDistilleryNameCacheService
            .Setup(nameCacheService => nameCacheService.TryGet(
                WhiskyBottleRequestTestData.AllValuesPopulated.DistilleryName,
                out distilleryNameDetails))
            .Returns(true);

        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper.Map(WhiskyBottleRequestTestData.AllValuesPopulated);

        Assert.Equal(Guid.AllBitsSet, whiskyBottle.DistilleryId);
    }

    [Fact]
    public void When_MapToEntityWithoutDistilleryNameInCache_Expect_EntityWithoutDistilleryIdSet()
    {
        DistilleryNameDetails? distilleryNameDetails = null;
        _mockDistilleryNameCacheService
            .Setup(nameCacheService => nameCacheService.TryGet(
                WhiskyBottleRequestTestData.AllValuesPopulated.DistilleryName,
                out distilleryNameDetails))
            .Returns(false);

        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper.Map(WhiskyBottleRequestTestData.AllValuesPopulated);

        Assert.Null(whiskyBottle.DistilleryId);
    }

    [Theory]
    [InlineData(BottleStatus.Unknown, "Unknown")]
    [InlineData(BottleStatus.Unopened, "Unopened")]
    [InlineData(BottleStatus.Opened, "Opened")]
    [InlineData(BottleStatus.Finished, "Finished")]
    public void When_MapToEntityWithKnownStatuses_Expect_EntityWithStatusSet(BottleStatus status, string expectedStatus)
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper
            .Map(WhiskyBottleRequestTestData.AllValuesPopulated with { Status = status });

        Assert.Equal(expectedStatus, whiskyBottle.Status);
    }

    [Fact]
    public void When_MapToEntityWithoutStatus_Expect_EntityWithoutStatusSetToUnknown()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper
            .Map(WhiskyBottleRequestTestData.AllValuesPopulated with { Status = null });

        Assert.Equal("Unknown", whiskyBottle.Status);
    }

    [Fact]
    public void When_MapToEntityWithoutRemainingVolumeClSet_Expect_EntityWithRemainingVolumeClSetToVolumeCl()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper
            .Map(WhiskyBottleRequestTestData.AllValuesPopulated with { VolumeRemainingCl = null });

        Assert.Equal(WhiskyBottleRequestTestData.AllValuesPopulated.VolumeCl, whiskyBottle.VolumeRemainingCl);
    }
}