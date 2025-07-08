using Microsoft.EntityFrameworkCore;
using Moq;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.Database.Tests.Resources.TestData;

namespace MyWhiskyShelf.Database.Tests.Services;

public class DistilleryWriteServiceTests
{
    [Fact]
    public async Task When_AddDistilleryAndDistilleryDoesNotExist_Expect_DatabaseContainsDistilleryEntity()
    {
        await using var dbContext = await CreateDbContextAsync();
        
        var distilleryWriteService = new DistilleryWriteService(
            dbContext, 
            new Mock<IDistilleryNameCacheService>().Object,
            new DistilleryMapper());
        var result = await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryTestData.Aberargie);
        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .FirstAsync(entity => entity.DistilleryName == DistilleryEntityTestData.Aberargie.DistilleryName);
        
        Assert.Multiple(
            () => Assert.True(result, "'result' should be true"),
            () => Assert.Equal(
                DistilleryEntityTestData.Aberargie with { Id = distilleryEntity.Id }, 
                distilleryEntity));
    }
    
    [Fact]
    public async Task When_AddDistilleryAndDistilleryExists_Expect_DatabaseIsNotUpdated()
    {
        await using var dbContext = await CreateDbContextAsync();
        await dbContext
            .Set<DistilleryEntity>()
            .AddAsync(DistilleryEntityTestData.Aberargie);
        await dbContext.SaveChangesAsync();
        
        var modifiedAberargieDistillery = DistilleryTestData.Aberargie with { Active = false };
        
        var distilleryWriteService = new DistilleryWriteService(
            dbContext, 
            new Mock<IDistilleryNameCacheService>().Object,
            new DistilleryMapper());
        var result = await distilleryWriteService.TryAddDistilleryAsync(modifiedAberargieDistillery);

        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .FirstAsync(entity => entity.DistilleryName == modifiedAberargieDistillery.DistilleryName);


        Assert.Multiple(
            () => Assert.False(result, "'result' should be false"),
            () => Assert.Equal(distilleryEntity, DistilleryEntityTestData.Aberargie));
    }
    
    [Fact]
    public async Task When_AddDistilleryAndDistilleryDoesNotExist_Expect_DistilleryNameCacheServiceIsUpdated()
    {
        await using var dbContext = await CreateDbContextAsync();
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService => nameCacheService.Add(DistilleryTestData.Aberargie.DistilleryName))
            .Verifiable(Times.Once);
        
        var distilleryWriteService = new DistilleryWriteService(
            dbContext, 
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        await distilleryWriteService.TryAddDistilleryAsync(DistilleryTestData.Aberargie);
        
        mockDistilleryNameCacheService.Verify();
    }
    
    private static async Task<MyWhiskyShelfDbContext> CreateDbContextAsync(params DistilleryEntity[] distilleryEntities)
    {
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var dbContext = new MyWhiskyShelfDbContext(options);
        
        dbContext.Set<DistilleryEntity>().AddRange(distilleryEntities);
        await dbContext.SaveChangesAsync();
        
        return dbContext;
    }
}