using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.Database.Tests.Resources.TestData;

namespace MyWhiskyShelf.Database.Tests.Services;

public class DistilleryNameCacheServiceTests
{

    [Fact]
    public void When_GetAllAsyncAndCacheIsNotLoaded_Expect_EmptyList()
    {
        var distilleryNameClassService = new DistilleryNameCacheService();

        var distilleryNames = distilleryNameClassService.GetAll();

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_GetAllAsyncAndCacheIsLoaded_Expect_AllDistilleryNames()
    {
        var dbContext = await CreateDbContext(DistilleryEntityTestData.Aberargie, DistilleryEntityTestData.Aberfeldy);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService.GetAll();

        Assert.Equal(
            [
                DistilleryEntityTestData.Aberargie.DistilleryName,
                DistilleryEntityTestData.Aberfeldy.DistilleryName
            ],
            distilleryNames);
    }

    [Fact]
    public void When_SearchAsyncAndCacheIsNotLoaded_Expect_EmptyList()
    {
        var distilleryNameClassService = new DistilleryNameCacheService();

        var distilleryNames = distilleryNameClassService.Search(DistilleryEntityTestData.Aberargie.DistilleryName);

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_SearchAsyncAndQueryStringIsNotFuzzyFoundInDistilleryName_Expect_EmptyList()
    {
        const string queryString = "balir";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService.Search(queryString);

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_SearchAsyncAndQueryStringIsLessThanThreeCharacters_Expect_EmptyList()
    {
        const string queryString = "Ab";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService.Search(queryString);

        Assert.Empty(distilleryNames);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \n")]
    public async Task When_SearchAsyncAndQueryStringIsEmptyOrWhitespace_Expect_EmptyList(string queryString)
    {
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService.Search(queryString);

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_SearchAsyncAndQueryStringMatchesExactly_Expect_OnlyThatDistilleryNameReturned()
    {
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy,
            DistilleryEntityTestData.Balbalir,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService.Search(DistilleryEntityTestData.Aberfeldy.DistilleryName);

        Assert.Multiple(
            () => Assert.Single(distilleryNames),
            () => Assert.Equal(DistilleryEntityTestData.Aberfeldy.DistilleryName, distilleryNames.First()));
    }

    [Fact]
    public async Task When_SearchAsyncAndQueryStringMatchesStartOfDistilleryName_Expect_MatchingDistilleryNamesReturned()
    {
        const string queryString = "Aber";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy,
            DistilleryEntityTestData.Balbalir,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService.Search(queryString);

        Assert.Equal(
            [DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberfeldy.DistilleryName],
            distilleryNames);
    }

    [Fact]
    public async Task When_SearchAsyncAndDistilleryNameContainsQueryString_Expect_MatchingDistilleryNamesReturned()
    {
        const string queryString = "hai";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService
            .Search(queryString);

        Assert.Equal(
            [
                DistilleryEntityTestData.AbhainnDearg.DistilleryName,
                DistilleryEntityTestData.Bunnahabhain.DistilleryName
            ],
            distilleryNames);
    }

    [Fact]
    public async Task When_SearchAsyncAndDistilleryContainsFuzzyQueryString_Expect_FuzzyMatchingDistilleryNamesReturned()
    {
        const string queryString = "Abergie";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameClassService.Search(queryString);

        Assert.Single(distilleryNames);
        Assert.Equal([DistilleryEntityTestData.Aberargie.DistilleryName], distilleryNames);
    }

    [Fact]
    public void When_AddAndNameIsNotInCache_Expect_NameAddedToCache()
    {
        var distilleryNameClassService = new DistilleryNameCacheService();

        distilleryNameClassService.Add(DistilleryEntityTestData.Aberargie.DistilleryName);
        
        var result = distilleryNameClassService.Search(DistilleryTestData.Aberargie.DistilleryName);
        
        Assert.Equal(DistilleryEntityTestData.Aberargie.DistilleryName, result.First());
    }
    
    [Fact]
    public async Task When_AddAndNameIsInCache_Expect_NameIsNotAddedAgainToCache()
    {
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameClassService = new DistilleryNameCacheService();
        await distilleryNameClassService.LoadCacheFromDbAsync(dbContext);
        
        distilleryNameClassService.Add(DistilleryEntityTestData.Aberargie.DistilleryName);
        
        var result = distilleryNameClassService.Search(DistilleryTestData.Aberargie.DistilleryName);

        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(DistilleryEntityTestData.Aberargie.DistilleryName, result.First()));
    }
    
    private static async Task<MyWhiskyShelfDbContext> CreateDbContext(params DistilleryEntity[] distilleryNames)
    {
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new MyWhiskyShelfDbContext(options);

        dbContext.Set<DistilleryEntity>().AddRange(distilleryNames);
        await dbContext.SaveChangesAsync();

        return dbContext;
    }
}