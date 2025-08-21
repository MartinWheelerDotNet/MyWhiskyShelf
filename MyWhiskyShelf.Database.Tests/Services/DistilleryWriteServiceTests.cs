using Microsoft.EntityFrameworkCore;
using Moq;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.Database.Tests.TestHelpers;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Services;

public class DistilleryWriteServiceTests
{
    #region TryAdd Tests
    
    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryDoesNotExist_Expect_DatabaseContainsDistilleryEntity()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<DistilleryEntity>();

        var distilleryWriteService = CreateDistilleryWriteService(dbContext);
        var (hasBeenAdded, id) = await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie);
        
        Assert.True(hasBeenAdded, "'hasBeenAdded' should be true");
        await AssertDatabaseContainsEntity(dbContext, DistilleryEntityTestData.Aberargie, id!.Value);
    }

    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryExists_Expect_DoesNotAddToDatabase()
    {
        var mockDistilleryNameCacheService = MockDistilleryNameCacheServiceGetReturns(true);

        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        var (hasBeenAdded, _) = await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie);

        Assert.False(hasBeenAdded, "'hasBeenAdded' should be false");
    }

    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryExists_Expect_DistilleryNameCacheServiceIsNotUpdated()
    {
        var mockDistilleryNameCacheService = MockDistilleryNameCacheServiceGetReturns(true);
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<DistilleryEntity>();

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        await distilleryWriteService.TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie);

        mockDistilleryNameCacheService.Verify(
                nameCacheService => nameCacheService.Add(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Never);
    }

    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryDoesNotExist_Expect_DistilleryNameCacheServiceIsUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<DistilleryEntity>();

        var mockDistilleryNameCacheService = MockDistilleryNameCacheServiceGetReturns(false);
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        await distilleryWriteService.TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie);

        mockDistilleryNameCacheService.Verify(
            nameCacheService => nameCacheService.Add(DistilleryRequestTestData.Aberargie.Name, It.IsAny<Guid>()),
            Times.Once);
    }
    
    [Theory]
    [InlineData(typeof(DbUpdateException))]
    [InlineData(typeof(DbUpdateConcurrencyException))]
    [InlineData(typeof(Exception))]
    public async Task When_TryAddDistilleryAndDatabaseThrowsAnException_Expect_ReturnsFalse(Type exceptionType)
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateFailingDbContextAsync<DistilleryEntity>(exceptionType);

        var distilleryWriteService = CreateDistilleryWriteService(dbContext);
        var (hasBeenAdded, id) = await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie);

        Assert.Multiple(
                () => Assert.False(hasBeenAdded),
                () => Assert.Null(id));
    }
    #endregion

    #region TryUpdate Tests
    
    [Fact]
    public async Task When_TryUpdateDistilleryAndDistilleryExists_Expect_TrueAndDistilleryUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Founded = 2000 };
        
        var distilleryWriteService =  CreateDistilleryWriteService(dbContext);
        var hasBeenUpdated = await distilleryWriteService
            .TryUpdateDistilleryAsync(DistilleryEntityTestData.Aberargie.Id, distilleryRequest);
        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .FindAsync(DistilleryEntityTestData.Aberargie.Id);
        
        Assert.Multiple(
            () => Assert.True(hasBeenUpdated),
            () => Assert.Equal(2000,  distilleryEntity!.Founded));
    }
    
    [Fact]
    public async Task When_TryUpdateDistilleryWithDistilleryWithDifferentName_Expect_DistilleryNameCacheUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Name = "New Name" };
        
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        var hasBeenUpdated = await distilleryWriteService
            .TryUpdateDistilleryAsync(DistilleryEntityTestData.Aberargie.Id, distilleryRequest);
        
        Assert.Multiple(
            () => Assert.True(hasBeenUpdated),
            () => mockDistilleryNameCacheService
                .Verify(service => service.Remove(DistilleryEntityTestData.Aberargie.Id), Times.Once),
            () => mockDistilleryNameCacheService
                .Verify(service => service.Add("New Name", DistilleryEntityTestData.Aberargie.Id), Times.Once));
    }
    
    [Fact]
    public async Task When_TryUpdateDistilleryWithDistilleryWithSameName_Expect_DistilleryNameCacheNotUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Founded = 2000 };
        
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        var hasBeenUpdated = await distilleryWriteService
            .TryUpdateDistilleryAsync(DistilleryEntityTestData.Aberargie.Id, distilleryRequest);
        
        Assert.Multiple(
            () => Assert.True(hasBeenUpdated),
            () => mockDistilleryNameCacheService
                .Verify(service => service.Remove(DistilleryEntityTestData.Aberargie.Id), Times.Never),
            () => mockDistilleryNameCacheService
                .Verify(service => service.Add(It.IsAny<string>(), DistilleryEntityTestData.Aberargie.Id), Times.Never));
    }
    
    [Fact]
    public async Task When_TryUpdateDistilleryAndDistilleryDoesNotExist_Expect_False()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<DistilleryEntity>();
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Founded = 2000 };
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext);
        var hasBeenUpdated = await distilleryWriteService
            .TryUpdateDistilleryAsync(DistilleryEntityTestData.Aberargie.Id, distilleryRequest);
        
        Assert.False(hasBeenUpdated);
    }
    
    [Theory]
    [InlineData(typeof(DbUpdateException))]
    [InlineData(typeof(DbUpdateConcurrencyException))]
    [InlineData(typeof(Exception))]
    public async Task When_TryUpdateAndDatabaseThrowsException_Expect_ReturnsFalse(Type exceptionType)
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateFailingDbContextAsync(exceptionType, DistilleryEntityTestData.Aberargie);
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Founded = 2000 };
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext);
        var hasBeenUpdated = await distilleryWriteService
            .TryUpdateDistilleryAsync(DistilleryEntityTestData.Aberargie.Id, distilleryRequest);
        
        Assert.False(hasBeenUpdated);
    }
    #endregion
    
    #region TryRemove Tests
    [Fact]
    public async Task When_TryRemoveAndDistilleryExists_Expect_DistilleryIsRemoved()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var mockDistilleryNameCacheService = MockDistilleryNameCacheServiceGetReturns(true);
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        await distilleryWriteService.RemoveDistilleryAsync(DistilleryEntityTestData.Aberargie.Id);

        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .FindAsync(DistilleryEntityTestData.Aberargie.Id);
            
        Assert.Null(distilleryEntity);
    }

    [Fact]
    public async Task When_TryRemoveAndDistilleryExists_Expect_DistilleryNameCacheServiceIsUpdated()
    {
        await using var dbContext =
            await MyWhiskyShelfContextBuilder.CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var mockDistilleryNameCacheService = MockDistilleryNameCacheServiceGetReturns(true);

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        await distilleryWriteService.RemoveDistilleryAsync(DistilleryEntityTestData.Aberargie.Id);

        mockDistilleryNameCacheService
            .Verify(nameCacheService => nameCacheService.Remove(DistilleryEntityTestData.Aberargie.Id), Times.Once);
    }
    
    [Fact]
    public async Task When_TryRemoveAndDistilleryDoesNotExist_Expect_DistilleryNameCacheIsNotUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<DistilleryEntity>();

        var mockDistilleryNameCacheService = MockDistilleryNameCacheServiceGetReturns(false);
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        
        await distilleryWriteService.RemoveDistilleryAsync(
            DistilleryEntityTestData.Aberargie.Id);

        mockDistilleryNameCacheService
            .Verify(nameCacheService => nameCacheService.Remove(DistilleryEntityTestData.Aberargie.Id), Times.Never);
    }
    #endregion
    
    private static DistilleryWriteService CreateDistilleryWriteService(
        MyWhiskyShelfDbContext dbContext,
        IDistilleryNameCacheService? distilleryNameCacheService = null)
    {
        return new DistilleryWriteService(
            dbContext,
            distilleryNameCacheService ?? new Mock<IDistilleryNameCacheService>().Object,
            new DistilleryRequestToEntityMapper());
    }
    
    private static Mock<IDistilleryNameCacheService> MockDistilleryNameCacheServiceGetReturns(bool returnValue)
    {
        DistilleryNameDetails? distilleryDetails;
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(service => service.TryGet(It.IsAny<string>(), out distilleryDetails))
            .Returns(returnValue);
        return mockDistilleryNameCacheService;
    }

    private static async Task AssertDatabaseContainsEntity(
        MyWhiskyShelfDbContext dbContext, 
        DistilleryEntity expectedEntity,
        Guid distilleryId) 
    {
        expectedEntity.Id = distilleryId;
        var distilleryEntity = await dbContext.Set<DistilleryEntity>().FindAsync(distilleryId);
        Assert.Equivalent(expectedEntity, distilleryEntity);
    } 
}