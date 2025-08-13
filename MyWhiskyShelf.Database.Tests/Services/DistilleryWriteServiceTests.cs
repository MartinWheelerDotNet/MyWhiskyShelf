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
    [Fact]
    public async Task When_TryAddDistilleryAndIdempotencyKeyIsNotFound_Expect_AddedToDatabase()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<DistilleryEntity>();
        
        var mockIdempotencyService = new Mock<IIdempotencyService>();
        mockIdempotencyService
            .Setup(service => service.TryGetCachedResult(It.IsAny<Guid>()))
            .ReturnsAsync(await Task.FromResult<Guid?>(null));

        var distilleryWriteService = CreateDistilleryWriteService(
            dbContext,
            idempotencyService: mockIdempotencyService.Object);
        
        var (hasBeenAdded, _) = await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie, Guid.NewGuid());
        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .FirstAsync(entity => entity.Name == DistilleryEntityTestData.Aberargie.Name);

        var expectedDistilleryEntity = DistilleryEntityTestData.Aberargie;
        expectedDistilleryEntity.Id = distilleryEntity.Id;

        Assert.Multiple(
            () => Assert.True(hasBeenAdded, "'hasBeenAdded' should be true"),
            () => Assert.Equivalent(expectedDistilleryEntity, distilleryEntity));
    }
    
    [Fact]
    public async Task When_TryAddDistilleryAndIdempotencyKeyIsFound_Expect_NotAddedToDatabase()
    {
        var cachedDistilleryId = Guid.NewGuid();
        
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<DistilleryEntity>();
        
        var mockIdempotencyService = new Mock<IIdempotencyService>();
        mockIdempotencyService
            .Setup(service => service.TryGetCachedResult(It.IsAny<Guid>()))
            .ReturnsAsync(await Task.FromResult<Guid?>(cachedDistilleryId));

        var distilleryWriteService = CreateDistilleryWriteService(
            dbContext,
            idempotencyService: mockIdempotencyService.Object);
        
        var (hasBeenAdded, id) = await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie, Guid.NewGuid());
        var distilleryEntities = await dbContext.Set<DistilleryEntity>().ToListAsync();

        Assert.Multiple(
            () => Assert.True(hasBeenAdded, "'hasBeenAdded' should be true"),
            () => Assert.Equal(cachedDistilleryId, id),
            () => Assert.Empty(distilleryEntities));
    }

   

    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryDoesNotExist_Expect_DatabaseContainsDistilleryEntity()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<DistilleryEntity>();

        var distilleryWriteService = CreateDistilleryWriteService(dbContext);
        var (hasBeenAdded, _) = await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie, Guid.NewGuid());
        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .FirstAsync(entity => entity.Name == DistilleryEntityTestData.Aberargie.Name);

        var expectedDistilleryEntity = DistilleryEntityTestData.Aberargie;
        expectedDistilleryEntity.Id = distilleryEntity.Id;

        Assert.Multiple(
            () => Assert.True(hasBeenAdded, "'hasBeenAdded' should be true"),
            () => Assert.Equivalent(expectedDistilleryEntity, distilleryEntity));
    }

    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryExists_Expect_DatabaseIsNotUpdated()
    {
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        DistilleryNameDetails? distilleryDetails;
        mockDistilleryNameCacheService
            .Setup(service => service.TryGet(DistilleryEntityTestData.Aberargie.Name, out distilleryDetails))
            .Returns(true);

        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var aberargieDistillery = DistilleryRequestTestData.Aberargie with { Active = false };

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        var (hasBeenAdded, _) = await distilleryWriteService
            .TryAddDistilleryAsync(aberargieDistillery, Guid.NewGuid());

        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .FirstAsync(entity => entity.Name == aberargieDistillery.Name);

        Assert.Multiple(
            () => Assert.False(hasBeenAdded, "'hasBeenAdded' should be false"),
            () => Assert.Equivalent(distilleryEntity, DistilleryEntityTestData.Aberargie));
    }

    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryExists_Expect_DistilleryNameCacheServiceIsNotUpdated()
    {
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService =>
                nameCacheService.Add(DistilleryRequestTestData.Aberargie.Name, Guid.NewGuid()))
            .Verifiable(Times.Never);

        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie, Guid.NewGuid());

        mockDistilleryNameCacheService.Verify();
    }

    [Fact]
    public async Task When_TryAddDistilleryAndDistilleryDoesNotExist_Expect_DistilleryNameCacheServiceIsUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<DistilleryEntity>();

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService =>
                nameCacheService.Add(DistilleryRequestTestData.Aberargie.Name, It.IsAny<Guid>()))
            .Verifiable(Times.Once);

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        await distilleryWriteService
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie, Guid.NewGuid());

        mockDistilleryNameCacheService.Verify();
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
            .TryAddDistilleryAsync(DistilleryRequestTestData.Aberargie,  Guid.NewGuid());

        Assert.Multiple(
                () => Assert.False(hasBeenAdded),
                () => Assert.Null(id));
    }

    [Fact]
    public async Task When_TryRemoveDistilleryAndDistilleryExists_Expect_DistilleryIsRemoved()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService =>
                nameCacheService.Add(DistilleryRequestTestData.Aberargie.Name, Guid.NewGuid()))
            .Verifiable(Times.Once);

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        var hasBeenRemoved = await distilleryWriteService.TryRemoveDistilleryAsync(DistilleryEntityTestData.Aberargie.Id);

        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .ToListAsync();

        Assert.Multiple(
            () => Assert.True(hasBeenRemoved),
            () => Assert.Empty(distilleryEntity));
    }

    [Fact]
    public async Task When_TryRemoveDistilleryAndDistilleryExists_Expect_DistilleryNameCacheServiceIsUpdated()
    {
        await using var dbContext =
            await MyWhiskyShelfContextBuilder.CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService => nameCacheService.Remove(DistilleryEntityTestData.Aberargie.Id))
            .Verifiable(Times.Once);

        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        await distilleryWriteService.TryRemoveDistilleryAsync(DistilleryEntityTestData.Aberargie.Id);

        mockDistilleryNameCacheService.Verify();
    }
    
    [Fact]
    public async Task When_TryRemoveDistilleryAndDistilleryDoesNotExist_Expect_False()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<DistilleryEntity>();

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        
        var hasBeenRemoved = await distilleryWriteService.TryRemoveDistilleryAsync(
            DistilleryEntityTestData.Aberargie.Id);

        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .ToListAsync();

        Assert.Multiple(
            () => Assert.False(hasBeenRemoved),
            () => Assert.Empty(distilleryEntity));
    }

    [Fact]
    public async Task When_TryUpdateDistilleryAndDistilleryExists_Expect_TrueAndDistilleryUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync(
            DistilleryEntityTestData.Aberargie);
        
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        
        var distilleryWriteService =CreateDistilleryWriteService(dbContext, mockDistilleryNameCacheService.Object);
        
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Founded = 2000 };
        
        var hasBeenUpdated = await distilleryWriteService.TryUpdateDistilleryAsync(
            DistilleryEntityTestData.Aberargie.Id, 
            distilleryRequest);

        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .SingleAsync(entity => entity.Id == DistilleryEntityTestData.Aberargie.Id);
        
        Assert.True(hasBeenUpdated);
        Assert.Equal(2000,  distilleryEntity.Founded);
    }
    
    [Fact]
    public async Task When_TryUpdateDistilleryAndDistilleryDoesNotExist_Expect_False()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateDbContextAsync<DistilleryEntity>();
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext);
        
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Founded = 2000 };
        
        var hasBeenUpdated = await distilleryWriteService.TryUpdateDistilleryAsync(
            DistilleryEntityTestData.Aberargie.Id, 
            distilleryRequest);
        
        Assert.False(hasBeenUpdated);
    }

    [Theory]
    [InlineData(typeof(DbUpdateException))]
    [InlineData(typeof(DbUpdateConcurrencyException))]
    [InlineData(typeof(Exception))]
    public async Task When_TryUpdateAndDatabaseThrowsException_Expect_ReturnsFalse(Type exceptionType)
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder.CreateFailingDbContextAsync(
            exceptionType,
            DistilleryEntityTestData.Aberargie);
        
        
        var distilleryWriteService = CreateDistilleryWriteService(dbContext);
        
        var distilleryRequest = DistilleryRequestTestData.Aberargie with { Founded = 2000 };
        
        var hasBeenUpdated = await distilleryWriteService.TryUpdateDistilleryAsync(
            DistilleryEntityTestData.Aberargie.Id, 
            distilleryRequest);
        
        Assert.False(hasBeenUpdated);
    }
    
    private static DistilleryWriteService CreateDistilleryWriteService(
        MyWhiskyShelfDbContext dbContext,
        IDistilleryNameCacheService? distilleryNameCacheService = null,
        IIdempotencyService? idempotencyService = null)
    {
        return new DistilleryWriteService(
            dbContext,
            distilleryNameCacheService ?? new Mock<IDistilleryNameCacheService>().Object,
            idempotencyService ?? new Mock<IIdempotencyService>().Object,
            new DistilleryRequestToEntityMapper());
    }
}