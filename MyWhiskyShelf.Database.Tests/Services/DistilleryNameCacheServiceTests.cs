using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
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

        var distilleryDetails = distilleryNameCacheService.GetAll();

        Assert.Empty(distilleryDetails);
    }

    [Fact]
    public async Task When_GetAllAsyncAndCacheIsLoaded_Expect_AllDistilleryDetails()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberargie.Id),
            new(DistilleryEntityTestData.Aberfeldy.DistilleryName, DistilleryEntityTestData.Aberfeldy.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie, 
            DistilleryEntityTestData.Aberfeldy);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);

        var distilleryDetails = distilleryNameCacheService.GetAll();

        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }
    
    [Fact]
    public async Task When_GetAllAsyncAndCacheIsLoaded_Expect_AllDistilleryDetailsAreReturnedInAlphabeticalOrder()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberargie.Id),
            new(DistilleryEntityTestData.AbhainnDearg.DistilleryName, DistilleryEntityTestData.AbhainnDearg.Id),
            new(DistilleryEntityTestData.Balbalir.DistilleryName, DistilleryEntityTestData.Balbalir.Id),
            new(DistilleryEntityTestData.Bunnahabhain.DistilleryName, DistilleryEntityTestData.Bunnahabhain.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Balbalir,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.AbhainnDearg);
        
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);

        var distilleryDetails = distilleryNameCacheService.GetAll();

        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }

    [Fact]
    public void When_SearchAsyncAndCacheIsNotLoaded_Expect_EmptyList()
    {
        var distilleryNameCacheService = new DistilleryNameCacheService();

        var distilleryDetails = distilleryNameCacheService.Search(DistilleryEntityTestData.Aberargie.DistilleryName);

        Assert.Empty(distilleryDetails);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndQueryStringIsNotFuzzyFoundInDistilleryName_Expect_EmptyList()
    {
        const string queryString = "balir";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryDetails = distilleryNameCacheService.Search(queryString);
    
        Assert.Empty(distilleryDetails);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndQueryStringIsLessThanThreeCharacters_Expect_EmptyList()
    {
        const string queryString = "Ab";
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryDetails = distilleryNameCacheService.Search(queryString);
    
        Assert.Empty(distilleryDetails);
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
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryDetails = distilleryNameCacheService.Search(queryString);
    
        Assert.Empty(distilleryDetails);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndQueryStringMatchesExactly_Expect_OnlyThoseDistilleryDetailsAreReturned()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberfeldy.DistilleryName, DistilleryEntityTestData.Aberfeldy.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy,
            DistilleryEntityTestData.Balbalir,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryDetails = distilleryNameCacheService.Search(DistilleryEntityTestData.Aberfeldy.DistilleryName);
    
        Assert.Multiple(
            () => Assert.Single(distilleryDetails),
            () => Assert.Equal(expectedDistilleryDetails, distilleryDetails));
    }
    
    [Fact]
    public async Task When_SearchAsyncAndQueryStringMatchesStartOfDistilleryName_Expect_MatchingDistilleryDetailsReturned()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberargie.Id),
            new(DistilleryEntityTestData.Aberfeldy.DistilleryName, DistilleryEntityTestData.Aberfeldy.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.Aberfeldy,
            DistilleryEntityTestData.Balbalir,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryDetails = distilleryNameCacheService.Search("Aber");
    
        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndDistilleryNameContainsQueryString_Expect_MatchingDistilleryDetailsReturned()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.AbhainnDearg.DistilleryName, DistilleryEntityTestData.AbhainnDearg.Id),
            new(DistilleryEntityTestData.Bunnahabhain.DistilleryName, DistilleryEntityTestData.Bunnahabhain.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryDetails = distilleryNameCacheService.Search( "hai");
    
        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndDistilleryNameContainsQueryString_Expect_DistilleryDetailsInNameOrder()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.AbhainnDearg.DistilleryName, DistilleryEntityTestData.AbhainnDearg.Id),
            new(DistilleryEntityTestData.Bunnahabhain.DistilleryName, DistilleryEntityTestData.Bunnahabhain.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryNames = distilleryNameCacheService.Search("hai");
    
        Assert.Equal(expectedDistilleryDetails, distilleryNames);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndDistilleryContainsFuzzyQueryString_Expect_FuzzyMatchingDistilleryDetailsReturned()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberargie.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        var distilleryDetails = distilleryNameCacheService.Search("Abergie");
    
        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }
    
    [Fact]
    public async Task When_SearchAsyncAndTwoDistilleriesContainsFuzzyQueryString_Expect_FuzzyMatchedDistilleryDetailsReturned()
    {
        
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.AbhainnDearg.DistilleryName, DistilleryEntityTestData.AbhainnDearg.Id),
            new(DistilleryEntityTestData.Bunnahabhain.DistilleryName, DistilleryEntityTestData.Bunnahabhain.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Bunnahabhain,
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg
            );
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
    
        // in this case 'hainn' most closely matches 'AbhainnDearg', but also fuzzy matches to the 'hain' in
        // 'Bunnahabhain'.
        var distilleryDetails = distilleryNameCacheService.Search("hainn");
    
        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }
    
    [Fact]
    public void When_AddAndNameIsNotInCache_Expect_DistilleryDetailsAddedToCache()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberargie.Id)
        ];
        
        var distilleryNameCacheService = new DistilleryNameCacheService();
    
        distilleryNameCacheService.Add(
            DistilleryEntityTestData.Aberargie.DistilleryName, 
            DistilleryEntityTestData.Aberargie.Id);
        
        var distilleryDetails = distilleryNameCacheService.Search(DistilleryTestData.Aberargie.DistilleryName);
        
        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }
    
    [Fact]
    public async Task When_AddAndNameIsInCache_Expect_DistilleryDetailsAreNotAddedAgainToCache()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberargie.Id)
        ];
        
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg,
            DistilleryEntityTestData.Bunnahabhain);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
        
        distilleryNameCacheService.Add(
            DistilleryEntityTestData.Aberargie.DistilleryName,
            DistilleryEntityTestData.Aberargie.Id);
        
        var distilleryDetails = distilleryNameCacheService.Search(DistilleryTestData.Aberargie.DistilleryName);
    
        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }
    
    [Fact]
    public async Task When_RemoveAndNameIsInCache_Expect_DistilleryDetailsRemovedFromCache()
    {
        var dbContext = await CreateDbContext(
            DistilleryEntityTestData.Aberargie,
            DistilleryEntityTestData.AbhainnDearg);
        
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
        
        distilleryNameCacheService.Remove(DistilleryEntityTestData.Aberargie.DistilleryName);
        var distilleryDetails = distilleryNameCacheService.GetAll();
        
        Assert.DoesNotContain(
            distilleryDetails,
            details => details.DistilleryName == DistilleryEntityTestData.Aberargie.DistilleryName);
    }
    
    [Fact]
    public async Task When_RemoveAndNameIsInNotCache_Expect_CacheIsUnaltered()
    {
        List<DistilleryNameDetails> expectedDistilleryDetails =
        [
            new(DistilleryEntityTestData.Aberargie.DistilleryName, DistilleryEntityTestData.Aberargie.Id)
        ];
        var dbContext = await CreateDbContext(DistilleryEntityTestData.Aberargie);
        
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
        
        distilleryNameCacheService.Remove(DistilleryEntityTestData.Bunnahabhain.DistilleryName);
        var distilleryDetails = distilleryNameCacheService.GetAll();
    
        Assert.Equal(expectedDistilleryDetails, distilleryDetails);
    }

    [Fact]
    public async Task When_TryGetAndNameIsInCache_ExpectDistilleryDetailsReturned()
    {

        var expectedDistilleryDetails = new DistilleryNameDetails(
            DistilleryEntityTestData.Aberargie.DistilleryName,
            DistilleryEntityTestData.Aberargie.Id);
        
        var dbContext = await CreateDbContext(DistilleryEntityTestData.Aberargie);
        var distilleryNameCacheService = new DistilleryNameCacheService();
        await distilleryNameCacheService.InitializeFromDatabaseAsync(dbContext);
        
        var result = distilleryNameCacheService.TryGet(
            DistilleryEntityTestData.Aberargie.DistilleryName,
            out var distilleryDetails);

        Assert.Multiple(
            () => Assert.True(result),
            () => Assert.Equal(expectedDistilleryDetails, distilleryDetails));
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