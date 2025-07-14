using Moq;
using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Tests.Resources.TestData;

namespace MyWhiskyShelf.Database.Tests.Mappers;

public class WhiskyBottleMapperTests
{
    private readonly Mock<IDistilleryNameCacheService> _mockDistilleryNameCacheService = new();

    [Fact]
    public void When_MapToEntityWithAllValuesPopulated_Expect_EntityWithAllValuesPopulated()
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        
        var whiskyBottleEntity = whiskyBottleMapper.MapToEntity(WhiskyBottleTestData.AllValuesPopulated);
        
        Assert.Equal(WhiskyBottleEntityTestData.AllValuesPopulated, whiskyBottleEntity);
    }
    
    [Fact]
    public void When_MapToEntityWithDateBottledButNotYearBottled_Expect_EntityWithYearBottledSetFromDate()
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottleEntity = whiskyBottleMapper.MapToEntity(
            WhiskyBottleTestData.AllValuesPopulated with { YearBottled = null });
        
        Assert.Equal(WhiskyBottleEntityTestData.AllValuesPopulated.DateBottled?.Year, whiskyBottleEntity.YearBottled);
    }
    
    [Fact]
    public void When_MapToEntityWithoutDateBottledOrYearBottled_Expect_EntityWithoutYearBottledSet() {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottleEntity = whiskyBottleMapper.MapToEntity(
            WhiskyBottleTestData.AllValuesPopulated with { DateBottled = null, YearBottled = null });
        
        Assert.Null(whiskyBottleEntity.YearBottled);
    }

    [Fact]
    public void When_MapToEntityWithDistilleryNameInCache_Expect_EntityWithDistilleryIdSet()
    {
        var distilleryNameDetails = new DistilleryNameDetails("A Distillery Name", Guid.AllBitsSet);
       _mockDistilleryNameCacheService
           .Setup(nameCacheService => nameCacheService.TryGet(
               WhiskyBottleTestData.AllValuesPopulated.DistilleryName, 
               out distilleryNameDetails))
           .Returns(true);
       
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper.MapToEntity(WhiskyBottleTestData.AllValuesPopulated);
        
        Assert.Equal(Guid.AllBitsSet, whiskyBottle.DistilleryId);
    }
    
    [Fact]
    public void When_MapToEntityWithoutDistilleryNameInCache_Expect_EntityWithoutDistilleryIdSet()
    {
        DistilleryNameDetails? distilleryNameDetails = null;
        _mockDistilleryNameCacheService
            .Setup(nameCacheService => nameCacheService.TryGet(
                WhiskyBottleTestData.AllValuesPopulated.DistilleryName, 
                out distilleryNameDetails))
            .Returns(false);
       
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper.MapToEntity(WhiskyBottleTestData.AllValuesPopulated);
        
        Assert.Null(whiskyBottle.DistilleryId);
    }

    [Theory]
    [InlineData(BottleStatus.Unknown, "Unknown")]
    [InlineData(BottleStatus.Unopened, "Unopened")]
    [InlineData(BottleStatus.Opened, "Opened")]
    [InlineData(BottleStatus.Finished, "Finished")]
    public void When_MapToEntityWithKnownStatuses_Expect_EntityWithStatusSet(BottleStatus status, string expectedStatus)
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper
            .MapToEntity(WhiskyBottleTestData.AllValuesPopulated with { Status = status });
        
        Assert.Equal(expectedStatus, whiskyBottle.Status);
    }

    [Fact]
    public void When_MapToEntityWithoutStatus_Expect_EntityWithoutStatusSetToUnknown()
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper
            .MapToEntity(WhiskyBottleTestData.AllValuesPopulated with { Status = null });
        
        Assert.Equal("Unknown", whiskyBottle.Status);
    }
    
    [Fact]
    public void When_MapToEntityWithoutRemainingVolumeClSet_Expect_EntityWithRemainingVolumeClSetToVolumeCl()
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper
            .MapToEntity(WhiskyBottleTestData.AllValuesPopulated with { VolumeRemainingCl = null });
        
        Assert.Equal(WhiskyBottleTestData.AllValuesPopulated.VolumeCl, whiskyBottle.VolumeRemainingCl);
    }
    
    [Fact]
    public void When_MapToDomainWithAllValuesPopulated_Expect_DomainModelWithAllValuesPopulated()
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        
        var whiskyBottle = whiskyBottleMapper.MapToDomain(WhiskyBottleEntityTestData.AllValuesPopulated);
        
        Assert.Equal(WhiskyBottleTestData.AllValuesPopulated, whiskyBottle);
    }
    
    [Fact]
    public void When_MapToDomainWithDateBottledButNotYearBottled_Expect_DomainModelWithYearBottledSetFromDate()
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper.MapToDomain(
            WhiskyBottleEntityTestData.AllValuesPopulated with { YearBottled = null });
        
        Assert.Equal(WhiskyBottleEntityTestData.AllValuesPopulated.DateBottled?.Year, whiskyBottle.YearBottled);
    }
    
    [Fact]
    public void When_MapToDomainWithoutDateBottledOrYearBottled_Expect_DomainModelWithoutYearBottledSet() {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottleEntity = whiskyBottleMapper.MapToDomain(
            WhiskyBottleEntityTestData.AllValuesPopulated with { DateBottled = null, YearBottled = null });
        
        Assert.Null(whiskyBottleEntity.YearBottled);
    }
    
    [Theory]
    [InlineData("Unknown", BottleStatus.Unknown)]
    [InlineData("Unopened", BottleStatus.Unopened)]
    [InlineData("Opened", BottleStatus.Opened)]
    [InlineData("Finished", BottleStatus.Finished)]
    public void When_MapToDomainWithStatusStrings_Expect_EntityDomainModelStatusSet(
        string status, 
        BottleStatus expectedStatus)
    {
        var whiskyBottleMapper = new WhiskyBottleMapper(_mockDistilleryNameCacheService.Object);
        var whiskyBottle = whiskyBottleMapper
            .MapToDomain(WhiskyBottleEntityTestData.AllValuesPopulated with { Status = status });
        
        Assert.Equal(expectedStatus, whiskyBottle.Status);
    }
}