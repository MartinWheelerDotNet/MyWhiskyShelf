using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Models;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Tests.Contexts;

public class MyWhiskyShelfDbContextTests
{
    [Fact]
    public async Task When_GetAllDistilleriesAndNoDistilleriesAreFound_Expect_EmptyList()
    {
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        await using var dbContext = new MyWhiskyShelfDbContext(options);
        var distilleries = await dbContext.GetAllDistilleriesAsync();
        
        Assert.Empty(distilleries);
    }
    
    [Fact]
    public async Task When_GetAllDistilleriesAndDistilleriesAreFound_Expect_ListContainsMappedData()
    {
        var expectedDistilleries = new List<Distillery>
        {
            new()
            { 
                DistilleryName = "testDistilleryName1",
                Location = "testLocation1",
                Region = "testRegion1",
                Founded = 2024,
                Owner = "testOwner1",
                DistilleryType = "testDistilleryType1",
                Active = true
            }, 
            new() 
            {
                DistilleryName = "testDistilleryName2",
                Location = "testLocation2",
                Region = "testRegion2",
                Founded = 2025,
                Owner = "testOwner2",
                DistilleryType = "testDistilleryType2",
                Active = false
            }
        }; 
        
        var options = new DbContextOptionsBuilder<MyWhiskyShelfDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        await using var dbContext = new MyWhiskyShelfDbContext(options);
        await dbContext.Set<DistilleryEntity>().AddRangeAsync(
            new DistilleryEntity 
            { 
                DistilleryName = "testDistilleryName1",
                Location = "testLocation1",
                Region = "testRegion1",
                Founded = 2024,
                Owner = "testOwner1",
                DistilleryType = "testDistilleryType1",
                Active = true
            }, 
            new DistilleryEntity 
            {
                DistilleryName = "testDistilleryName2",
                Location = "testLocation2",
                Region = "testRegion2",
                Founded = 2025,
                Owner = "testOwner2",
                DistilleryType = "testDistilleryType2",
                Active = false
            });
        await dbContext.SaveChangesAsync();
        
        var distilleries = await dbContext.GetAllDistilleriesAsync();
        
        Assert.Equal(expectedDistilleries.Count, distilleries.Count);
    }
}