using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
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
            new DistilleryEntityToResponseMapper());
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();

        Assert.Empty(distilleries);
    }

    [Fact]
    public async Task When_GetAllDistilleriesAndDistilleriesAreFound_Expect_ListContainsMappedData()
    {
        List<DistilleryResponse> expectedDistilleryResponses =
        [
            DistilleryResponseTestData.Aberargie,
            DistilleryResponseTestData.Aberfeldy
        ];

        await using var dbContext = await CreateDbContextAsync(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new DistilleryEntityToResponseMapper());
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();

        Assert.All(expectedDistilleryResponses, distillery => Assert.Contains(distillery, distilleries));
    }

    [Fact]
    public async Task When_GetAllDistilleriesAndDistilleriesAreFound_Expect_ListIsOrderedByName()
    {
        List<DistilleryResponse> expectedDistilleryResponses =
        [
            DistilleryResponseTestData.Aberargie,
            DistilleryResponseTestData.Aberfeldy,
            DistilleryResponseTestData.Bunnahabhain
        ];

        await using var dbContext = await CreateDbContextAsync(
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new DistilleryEntityToResponseMapper());
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();

        Assert.Equal(expectedDistilleryResponses, distilleries);
    }

    [Fact]
    public async Task When_GetDistilleryByIdAndDistilleryIsNotFound_Expect_NoDistilleryReturned()
    {
        await using var dbContext = await CreateDbContextAsync();
        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new DistilleryEntityToResponseMapper());

        var distillery = await distilleryReadService
            .GetDistilleryByIdAsync(Guid.NewGuid());

        Assert.Null(distillery);
    }

    [Fact]
    public async Task When_GetDistilleryByIdAndDistilleryIsFound_Expect_DistilleryReturned()
    {
        await using var dbContext = await CreateDbContextAsync(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy);

        var distilleryReadService = new DistilleryReadService(
            dbContext,
            new DistilleryEntityToResponseMapper());
        var distillery = await distilleryReadService
            .GetDistilleryByIdAsync(DistilleryEntityTestData.Aberfeldy.Id);

        Assert.Equal(DistilleryResponseTestData.Aberfeldy, distillery);
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