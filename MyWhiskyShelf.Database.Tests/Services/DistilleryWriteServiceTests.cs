using Microsoft.EntityFrameworkCore;
using Moq;
using MyWhiskyShelf.Core.Models;
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
    public async Task When_AddDistilleryAndDistilleryDoesNotExist_Expect_DatabaseContainsDistilleryEntity()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<DistilleryEntity>();

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
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        DistilleryNameDetails? distilleryDetails;
        mockDistilleryNameCacheService
            .Setup(service => service.TryGet(DistilleryEntityTestData.Aberargie.DistilleryName, out distilleryDetails))
            .Returns(true);

        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var modifiedAberargieDistillery = DistilleryTestData.Aberargie with { Active = false };

        var distilleryWriteService = new DistilleryWriteService(
            dbContext,
            mockDistilleryNameCacheService.Object,
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
    public async Task When_AddDistilleryAndDistilleryExists_Expect_DistilleryNameCacheServiceIsNotUpdated()
    {
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService =>
                nameCacheService.Add(DistilleryTestData.Aberargie.DistilleryName, Guid.NewGuid()))
            .Verifiable(Times.Never);

        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var distilleryWriteService = new DistilleryWriteService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        await distilleryWriteService.TryAddDistilleryAsync(DistilleryTestData.Aberargie);

        mockDistilleryNameCacheService.Verify();
    }

    [Fact]
    public async Task When_AddDistilleryAndDistilleryDoesNotExist_Expect_DistilleryNameCacheServiceIsUpdated()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync<DistilleryEntity>();

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService =>
                nameCacheService.Add(DistilleryTestData.Aberargie.DistilleryName, It.IsAny<Guid>()))
            .Verifiable(Times.Once);

        var distilleryWriteService = new DistilleryWriteService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        await distilleryWriteService.TryAddDistilleryAsync(DistilleryTestData.Aberargie);

        mockDistilleryNameCacheService.Verify();
    }

    [Fact]
    public async Task When_RemoveDistilleryAndDistilleryExists_Expect_DistilleryIsRemoved()
    {
        await using var dbContext = await MyWhiskyShelfContextBuilder
            .CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService =>
                nameCacheService.Add(DistilleryTestData.Aberargie.DistilleryName, Guid.NewGuid()))
            .Verifiable(Times.Once);

        var distilleryWriteService = new DistilleryWriteService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        await distilleryWriteService.TryRemoveDistilleryAsync(DistilleryTestData.Aberargie.DistilleryName);

        var distilleryEntity = await dbContext
            .Set<DistilleryEntity>()
            .ToListAsync();

        Assert.Empty(distilleryEntity);
    }

    [Fact]
    public async Task When_RemoveDistilleryAndDistilleryExists_Expect_DistilleryNameCacheServiceIsUpdated()
    {
        await using var dbContext =
            await MyWhiskyShelfContextBuilder.CreateDbContextAsync(DistilleryEntityTestData.Aberargie);

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(nameCacheService => nameCacheService.Remove(DistilleryTestData.Aberargie.DistilleryName))
            .Verifiable(Times.Once);

        var distilleryWriteService = new DistilleryWriteService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        await distilleryWriteService.TryRemoveDistilleryAsync(DistilleryTestData.Aberargie.DistilleryName);

        mockDistilleryNameCacheService.Verify();
    }
}