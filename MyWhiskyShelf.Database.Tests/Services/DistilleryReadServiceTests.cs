using Microsoft.EntityFrameworkCore;
using Moq;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.Database.Tests.Resources;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.Database.Tests.Services;

public class DistilleryReadServiceTests
{
    [Fact]
    public async Task When_GetAllDistilleriesAndNoDistilleriesAreFound_Expect_EmptyList()
    {
        await using var dbContext = await CreateDbContextAsync();
        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new Mock<IDistilleryNameCacheService>().Object,
            new DistilleryMapper());
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();

        Assert.Empty(distilleries);
    }

    [Fact]
    public async Task When_GetAllDistilleriesAndDistilleriesAreFound_Expect_ListContainsMappedData()
    {
        List<Distillery> expectedDistilleries = [DistilleryTestData.Aberargie, DistilleryTestData.Aberfeldy];

        await using var dbContext = await CreateDbContextAsync(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new Mock<IDistilleryNameCacheService>().Object,
            new DistilleryMapper());
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();

        Assert.All(expectedDistilleries, distillery => Assert.Contains(distillery, distilleries));
    }

    [Fact]
    public async Task When_GetAllDistilleriesAndDistilleriesAreFound_Expect_ListIsOrderedByDistilleryName()
    {
        List<Distillery> expectedDistilleries =
        [
            DistilleryTestData.Aberargie,
            DistilleryTestData.Aberfeldy,
            DistilleryTestData.Bunnahabhain
        ];

        await using var dbContext = await CreateDbContextAsync(
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new Mock<IDistilleryNameCacheService>().Object,
            new DistilleryMapper());
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();

        Assert.All(expectedDistilleries, distillery => Assert.Contains(distillery, distilleries));
    }

    [Fact]
    public async Task When_GetDistilleryNamesAndNoDistilleriesAreFound_Expect_EmptyList()
    {
        await using var dbContext = await CreateDbContextAsync();
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(cacheService => cacheService.GetAll())
            .Returns([]);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        var distilleryNames = distilleryReadService.GetDistilleryNames();

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_GetDistilleryNamesAndDistilleriesAreFound_Expect_ListContainsDistilleryNames()
    {
        List<string> expectedDistilleryNames =
        [
            DistilleryEntityTestData.Aberargie.DistilleryName,
            DistilleryEntityTestData.Aberfeldy.DistilleryName
        ];

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(cacheService => cacheService.GetAll())
            .Returns([
                new DistilleryNameDetails(DistilleryEntityTestData.Aberargie.DistilleryName, Guid.NewGuid()),
                new DistilleryNameDetails(DistilleryEntityTestData.Aberfeldy.DistilleryName, Guid.NewGuid())
            ]);

        await using var dbContext = await CreateDbContextAsync(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        var distilleryNames = distilleryReadService.GetDistilleryNames();

        Assert.Equal(expectedDistilleryNames, distilleryNames);
    }

    [Fact]
    public async Task When_GetDistilleryByNameAndDistilleryIsNotFound_Expect_NoDistilleryReturned()
    {
        await using var dbContext = await CreateDbContextAsync();
        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new Mock<IDistilleryNameCacheService>().Object,
            new DistilleryMapper());

        var distillery = await distilleryReadService
            .GetDistilleryByNameAsync(DistilleryTestData.Aberfeldy.DistilleryName);

        Assert.Null(distillery);
    }

    [Fact]
    public async Task When_GetDistilleryByNameAndDistilleryIsFound_Expect_DistilleryReturned()
    {
        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        var distilleryNameDetails = new DistilleryNameDetails(
            DistilleryEntityTestData.Aberfeldy.DistilleryName,
            DistilleryEntityTestData.Aberfeldy.Id);

        mockDistilleryNameCacheService
            .Setup(service =>
                service.TryGet(DistilleryEntityTestData.Aberfeldy.DistilleryName, out distilleryNameDetails))
            .Returns(true);

        await using var dbContext = await CreateDbContextAsync(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());
        var distillery = await distilleryReadService
            .GetDistilleryByNameAsync(DistilleryTestData.Aberfeldy.DistilleryName);

        Assert.Equal(DistilleryTestData.Aberfeldy, distillery);
    }

    [Fact]
    public async Task When_SearchAndDistilleryNamesFound_Expect_ListContainsDistilleryNames()
    {
        const string searchPattern = "aber";
        List<string> expectedDistilleryNames =
        [
            DistilleryEntityTestData.Aberargie.DistilleryName,
            DistilleryEntityTestData.Aberfeldy.DistilleryName
        ];

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(cacheService => cacheService.Search(searchPattern))
            .Returns([
                new DistilleryNameDetails(DistilleryEntityTestData.Aberargie.DistilleryName, Guid.NewGuid()),
                new DistilleryNameDetails(DistilleryEntityTestData.Aberfeldy.DistilleryName, Guid.NewGuid())
            ]);

        await using var dbContext = await CreateDbContextAsync();

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());

        var distilleryNames = distilleryReadService.SearchByName(searchPattern);

        Assert.Equal(expectedDistilleryNames, distilleryNames);
    }

    [Fact]
    public async Task When_SearchAndNoDistilleryNamesFound_Expect_EmptyListReturned()
    {
        const string searchPattern = "aber";

        var mockDistilleryNameCacheService = new Mock<IDistilleryNameCacheService>();
        mockDistilleryNameCacheService
            .Setup(cacheService => cacheService.Search(searchPattern))
            .Returns([]);

        await using var dbContext = await CreateDbContextAsync();

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            mockDistilleryNameCacheService.Object,
            new DistilleryMapper());

        var distilleryNames = distilleryReadService.SearchByName(searchPattern);

        Assert.Empty(distilleryNames);
    }

    private static async Task<MyWhiskyShelfDbContext> CreateDbContextAsync(params DistilleryEntity[] distilleryEntities)
    {
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new MyWhiskyShelfDbContext(options);

        dbContext.Set<DistilleryEntity>().AddRange(distilleryEntities);
        await dbContext.SaveChangesAsync();

        return dbContext;
    }
}