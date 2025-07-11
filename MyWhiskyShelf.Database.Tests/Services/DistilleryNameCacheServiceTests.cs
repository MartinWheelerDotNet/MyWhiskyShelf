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
        var distilleryNameCacheService = new DistilleryNameCacheService();

        var distilleryNames = distilleryNameCacheService.GetAll();

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_GetAllAsyncAndCacheIsLoaded_Expect_AllDistilleryNames()
    {
        var dbContext = await CreateDbContext(DistilleryEntityTestData.Aberargie, DistilleryEntityTestData.Aberfeldy);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.GetAll();

        Assert.Equal(
            [
                DistilleryEntityTestData.Aberargie.DistilleryName,
                DistilleryEntityTestData.Aberfeldy.DistilleryName
            ],
            distilleryNames);
    }
    
    [Fact]
    public async Task When_GetAllAsyncAndCacheIsLoaded_Expect_AllDistilleryNamesAreReturnedInAlphabeticalOrder()
    {
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Balbalir,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.AbhainnDearg);
        
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.GetAll();

        Assert.Equal(
            [
                DistilleryEntityTestData.Aberargie.DistilleryName,
                DistilleryEntityTestData.AbhainnDearg.DistilleryName,
                DistilleryEntityTestData.Balbalir.DistilleryName,
                DistilleryEntityTestData.Bunnahabhain.DistilleryName
            ],
            distilleryNames);
    }

    [Fact]
    public void When_SearchAsyncAndCacheIsNotLoaded_Expect_EmptyList()
    {
        var distilleryNameCacheService = new DistilleryNameCacheService();

        var distilleryNames = distilleryNameCacheService.Search(DistilleryEntityTestData.Aberargie.DistilleryName);

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_SearchAsyncAndQueryStringIsNotFuzzyFoundInDistilleryName_Expect_EmptyList()
    {
        const string queryString = "balir";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.Search(queryString);

        Assert.Empty(distilleryNames);
    }

    [Fact]
    public async Task When_SearchAsyncAndQueryStringIsLessThanThreeCharacters_Expect_EmptyList()
    {
        const string queryString = "Ab";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.Search(queryString);

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
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.Search(queryString);

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
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.Search(DistilleryEntityTestData.Aberfeldy.DistilleryName);

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
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.Search(queryString);

        Assert.Equal([
                DistilleryEntityTestData.Aberargie.DistilleryName, 
                DistilleryEntityTestData.Aberfeldy.DistilleryName
            ],
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
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService
            .Search(queryString);

        Assert.Equal(
            [
                DistilleryEntityTestData.AbhainnDearg.DistilleryName,
                DistilleryEntityTestData.Bunnahabhain.DistilleryName
            ],
            distilleryNames);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndDistilleryNameContainsQueryString_Expect_DistilleryNamesInOrder()
    {
        const string queryString = "hai";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService
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
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.Search(queryString);

        Assert.Single(distilleryNames);
        Assert.Equal([DistilleryEntityTestData.Aberargie.DistilleryName], distilleryNames);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndTwoDistilleriesContainsFuzzyQueryString_Expect_FuzzyMatchedDistilleryNamesReturned()
    {
        // in this case 'hainn' most closely matches 'AbhainnDearg', but also fuzzy matches to the 'hain' in
        // 'Bunnahabhain'.
        const string queryString = "hainn";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg
            );
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);

        var distilleryNames = distilleryNameCacheService.Search(queryString);

        Assert.Equal(
            [
                DistilleryEntityTestData.AbhainnDearg.DistilleryName,
                DistilleryEntityTestData.Bunnahabhain.DistilleryName
            ],
            distilleryNames);
    }

    [Fact]
    public void When_AddAndNameIsNotInCache_Expect_NameAddedToCache()
    {
        var distilleryNameCacheService = new DistilleryNameCacheService();

        distilleryNameCacheService.Add(DistilleryEntityTestData.Aberargie.DistilleryName);
        
        var result = distilleryNameCacheService.Search(DistilleryTestData.Aberargie.DistilleryName);
        
        Assert.Equal(DistilleryEntityTestData.Aberargie.DistilleryName, result.First());
    }
    
    [Fact]
    public async Task When_AddAndNameIsInCache_Expect_NameIsNotAddedAgainToCache()
    {
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);
        
        distilleryNameCacheService.Add(DistilleryEntityTestData.Aberargie.DistilleryName);
        
        var result = distilleryNameCacheService.Search(DistilleryTestData.Aberargie.DistilleryName);

        Assert.Multiple(
            () => Assert.Single(result),
            () => Assert.Equal(DistilleryEntityTestData.Aberargie.DistilleryName, result.First()));
    }

    [Fact]
    public async Task When_RemoveAndNameIsInCache_Expect_NameDeletedFromCache()
    {
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg);
        
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);
        
        distilleryNameCacheService.Remove(DistilleryEntityTestData.Aberargie.DistilleryName);
        var distilleryNames = distilleryNameCacheService.GetAll();
        
        Assert.Multiple(
            () => Assert.Single(distilleryNames),
            () => Assert.DoesNotContain(DistilleryEntityTestData.Aberargie.DistilleryName, distilleryNames));
    }
    
    [Fact]
    public async Task When_RemoveAndNameIsInNotCache_Expect_CacheIsUnaltered()
    {
        List<string> expectedDistilleryNames = [DistilleryEntityTestData.Aberargie.DistilleryName];
        
        var dbContext = await CreateDbContext(DistilleryEntityTestData.Aberargie);
        
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.LoadCacheFromDbAsync(dbContext);
        
        distilleryNameCacheService.Remove(DistilleryEntityTestData.Bunnahabhain.DistilleryName);
        var distilleryNames = distilleryNameCacheService.GetAll();

        Assert.Equal(expectedDistilleryNames, distilleryNames);
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