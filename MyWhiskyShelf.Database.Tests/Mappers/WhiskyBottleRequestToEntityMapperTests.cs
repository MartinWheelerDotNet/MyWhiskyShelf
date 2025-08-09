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
        var expectedWhiskyBottleEntity = WhiskyBottleEntityTestData.AllValuesPopulated;
        expectedWhiskyBottleEntity.Id = Guid.Empty;

        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var whiskyBottleEntity = whiskyBottleMapper.Map(WhiskyBottleRequestTestData.AllValuesPopulated);

        Assert.Equivalent(expectedWhiskyBottleEntity, whiskyBottleEntity);
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
        var distilleryNameDetails = new DistilleryNameDetails("A Distillery Name", Guid.AllBitsSet);
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
    
    [Fact]
    public void When_StatusIsSet_Then_StatusIsReturnedAsStringNotDefaultUnknown()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var request = WhiskyBottleRequestTestData.AllValuesPopulated with { Status = BottleStatus.Opened };
        var result = whiskyBottleMapper.Map(request);

        Assert.Equal("Opened", result.Status);
    }

    [Fact]
    public void When_StatusIsNull_Then_StatusIsSetToUnknown()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var request = WhiskyBottleRequestTestData.AllValuesPopulated with { Status = null };
        var result = whiskyBottleMapper.Map(request);

        Assert.Equal("Unknown", result.Status);
    }

    [Fact]
    public void When_YearBottledIsSet_Then_YearBottledIsReturnedOverDateBottledYear()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var request = WhiskyBottleRequestTestData.AllValuesPopulated with
        {
            YearBottled = 2020,
            DateBottled = new DateOnly(2000, 1, 1)
        };
        var result = whiskyBottleMapper.Map(request);

        Assert.Equal(2020, result.YearBottled);
    }

    [Fact]
    public void When_YearBottledIsNull_Then_DateBottledYearIsReturned()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var request = WhiskyBottleRequestTestData.AllValuesPopulated with
        {
            YearBottled = null,
            DateBottled = new DateOnly(2000, 1, 1)
        };
        var result = whiskyBottleMapper.Map(request);

        Assert.Equal(2000, result.YearBottled);
    }

    [Fact]
    public void When_VolumeRemainingClIsSet_Then_VolumeRemainingClIsReturnedNotVolumeCl()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var request = WhiskyBottleRequestTestData.AllValuesPopulated with
        {
            VolumeCl = 100,
            VolumeRemainingCl = 50
        };
        var result = whiskyBottleMapper.Map(request);

        Assert.Equal(50, result.VolumeRemainingCl);
    }

    [Fact]
    public void When_VolumeRemainingClIsNull_Then_VolumeClIsUsedAsRemainingVolume()
    {
        var whiskyBottleMapper = new WhiskyBottleRequestToEntityMapper(_mockDistilleryNameCacheService.Object);

        var request = WhiskyBottleRequestTestData.AllValuesPopulated with
        {
            VolumeCl = 100,
            VolumeRemainingCl = null
        };
        var result = whiskyBottleMapper.Map(request);

        Assert.Equal(100, result.VolumeRemainingCl);
    }
}