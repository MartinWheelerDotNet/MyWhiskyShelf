using Microsoft.Extensions.Logging.Abstractions;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Tests;

public class DataLoaderTests
{
    [Fact]
    public async Task When_GetDistilleriesFromJson_WithFileNotFound_ExpectExceptionThrown()
    {
        const string filename = "./not-a-valid-file.csv";
        
        var dataLoader = new DataLoader(NullLogger<DataLoader>.Instance);
        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => dataLoader.GetDistilleriesFromJson(filename));
        
        Assert.Equal($"'{filename}' not found when loading distillery data.", exception.Message);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJson_WithEmptyFileProvided_ExpectExceptionThrown()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/empty-file.json");
        
        var dataLoader = new DataLoader(NullLogger<DataLoader>.Instance);
        var exception = await Assert.ThrowsAsync<InvalidDataException>(
            () => dataLoader.GetDistilleriesFromJson(filePath));
        
        Assert.Equal($"'{filePath}' is found, but empty, when loading distillery data.", exception.Message);
    }
    
    [Fact]
    public async Task When_GetDistilleriesFromJson_WithInvalidFormatProvided_ExpectExceptionThrown()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/invalid-format.json");
        
        var dataLoader = new DataLoader(NullLogger<DataLoader>.Instance);
        var exception = await Assert.ThrowsAsync<InvalidDataException>(
            () => dataLoader.GetDistilleriesFromJson(filePath));
        
        Assert.Equal($"'{filePath}' is found, but contains invalid data, when loading distillery data.", exception.Message);
    }
    
    [Fact]
    public async Task When_GetDistilleriesFromJson_WithNoDistilleryRecords_ExpectEmptyListOfDistilleryData()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/no-distillery-data.json");

        var dataLoader = new DataLoader(NullLogger<DataLoader>.Instance);
        var distilleries = await dataLoader.GetDistilleriesFromJson(filePath);

        Assert.Empty(distilleries);
    }
    
    [Fact]
    public async Task When_GetDistilleriesFromJson_WithOneDistillery_ExpectListOfJustThatDistillery()
    {
        var expectedDistillery = new Distillery
        {
            DistilleryName = "Aberargie",
            Location = "Aberargie",
            Region = "Lowland",
            Founded = 2017,
            Owner = "Perth Distilling Co",
            DistilleryType = "Malt",
            Active = true
        };
            
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/single-distillery.json");
        
        var dataLoader = new DataLoader(NullLogger<DataLoader>.Instance);
        var distilleries = await dataLoader.GetDistilleriesFromJson(filePath);

        var distillery = Assert.Single(distilleries);
        Assert.Equal(expectedDistillery, distillery);
    }
    
    [Fact]
    public async Task When_GetDistilleriesFromJson_WithThreeDistilleries_ExpectListOfJustThoseDistilleries()
    {
        var expectedDistilleries = new []
        {
            new Distillery
            {
                DistilleryName = "Aberargie",
                Location = "Aberargie",
                Region = "Lowland",
                Founded = 2017,
                Owner = "Perth Distilling Co",
                DistilleryType = "Malt",
                Active = true
            },
            new Distillery
            {
                DistilleryName = "Aberfeldy",
                Location = "Aberfeldy",
                Region = "Highland",
                Founded = 1896,
                Owner = "John Dewar & Sons",
                DistilleryType = "Malt",
                Active = true
            },
            new Distillery
            {
                DistilleryName = "Aberlour",
                Location = "Aberlour",
                Region = "Speyside",
                Founded = 1879,
                Owner = "Chivas Brothers",
                DistilleryType = "Malt",
                Active = true
            }
        };
        
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/three-distilleries.json");
        
        var dataLoader = new DataLoader(NullLogger<DataLoader>.Instance);
        var distilleries = await dataLoader.GetDistilleriesFromJson(filePath);

        Assert.Equal(3, expectedDistilleries.Length);
        Assert.All(distilleries, distillery => Assert.Contains(distillery, expectedDistilleries));
    }
}