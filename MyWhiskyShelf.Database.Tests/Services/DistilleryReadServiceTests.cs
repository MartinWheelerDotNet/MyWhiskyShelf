using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Models;
using MyWhiskyShelf.Database.Services;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Tests.Services;

public class DistilleryReadServiceTests
{
    private static readonly DistilleryEntity FirstDistilleryEntity = new()
    { 
        Id = Guid.NewGuid(),
        DistilleryName = "Aberargie",
        Location = "Aberargie",
        Region = "Lowland",
        Founded = 2017,
        Owner = "Perth Distilling Co",
        DistilleryType = "Single Malt",
        Active = true
    };
    
    private static readonly DistilleryEntity SecondDistilleryEntity = new()
    { 
        Id = Guid.NewGuid(),
        DistilleryName = "Aberfeldy",
        Location = "Aberfeldy",
        Region = "Highland",
        Founded = 1896,
        Owner = "John Dewar & Sons",
        DistilleryType = "Single Malt",
        Active = true
    };

    [Fact]
    public async Task When_GetAllDistilleriesAndNoDistilleriesAreFound_Expect_EmptyList()
    {
        await using var dbContext = CreateDbContext; 
        var distilleryReadService = new DistilleryReadService(dbContext);
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();
        
        Assert.Empty(distilleries);
    }
    
    [Fact]
    public async Task When_GetAllDistilleriesAndDistilleriesAreFound_Expect_ListContainsMappedData()
    {
        var expectedDistilleries = new List<Distillery>
        {
            new()
            { 
                DistilleryName = "Aberargie",
                Location = "Aberargie",
                Region = "Lowland",
                Founded = 2017,
                Owner = "Perth Distilling Co",
                DistilleryType = "Single Malt",
                Active = true
            }, 
            new() 
            {
                DistilleryName = "Aberfeldy",
                Location = "Aberfeldy",
                Region = "Highland",
                Founded = 1896,
                Owner = "John Dewar & Sons",
                DistilleryType = "Single Malt",
                Active = true
            }
        }; 
        
        await using var dbContext = CreateDbContext; 
        await dbContext.Set<DistilleryEntity>().AddRangeAsync(FirstDistilleryEntity, SecondDistilleryEntity);
        await dbContext.SaveChangesAsync();

        
        var distilleryReadService = new DistilleryReadService(dbContext);
        var distilleries = await distilleryReadService.GetAllDistilleriesAsync();
        
        Assert.True(expectedDistilleries.ToHashSet().SetEquals(distilleries));
    }

    [Fact]
    public async Task When_GetDistilleryNamesAndNoDistilleriesAreFound_Expect_EmptyList()
    {
        await using var dbContext = CreateDbContext; 
        
        var distilleryReadService = new DistilleryReadService(dbContext);
        var distilleryNames = await distilleryReadService.GetAllDistilleriesAsync();

        Assert.Empty(distilleryNames);
    }
    
    [Fact]
    public async Task When_GetDistilleryNamesAndDistilleriesAreFound_Expect_ListContainsDistilleryNames()
    {
        List<string> expectedDistilleryNames = ["Aberargie", "Aberfeldy"];
        
        await using var dbContext = CreateDbContext;
        await dbContext.Set<DistilleryEntity>().AddRangeAsync(
            FirstDistilleryEntity, SecondDistilleryEntity);
        await dbContext.SaveChangesAsync();

        
        var distilleryReadService = new DistilleryReadService(dbContext);
        var distilleryNames = await distilleryReadService.GetDistilleryNamesAsync();

        Assert.True(expectedDistilleryNames.ToHashSet().SetEquals(distilleryNames));
    }
    
    private static MyWhiskyShelfDbContext CreateDbContext => new(
        new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options);

}