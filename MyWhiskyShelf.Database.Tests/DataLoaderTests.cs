namespace MyWhiskyShelf.Database.Tests;

public class DataLoaderTests
{
    [Fact]
    public async Task When_GetDistilleryData_WithFileNotFound_ExpectExceptionThrown()
    {
        const string filename = "./not-a-valid-file.csv";
        var dataLoader = new DataLoader();

        var exception = await Assert.ThrowsAsync<FileNotFoundException>(
            () => dataLoader.GetDistilleryData(filename));
        
        Assert.Equal($"'{filename}' not found when loading distillery data.", exception.Message);
    }

    [Fact]
    public async Task When_GetDistilleryData_WithEmptyFileProvided_ExpectExceptionThrown()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/empty-file.json");
        
        var dataLoader = new DataLoader();
        var exception = await Assert.ThrowsAsync<InvalidDataException>(
            () => dataLoader.GetDistilleryData(filePath));
        
        Assert.Equal($"'{filePath}' is found, but empty, when loading distillery data.", exception.Message);
    }
    
    [Fact]
    public async Task When_GetDistilleryData_WithInvalidFormatProvided_ExpectExceptionThrown()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/invalid-format.json");
        
        var dataLoader = new DataLoader();
        var exception = await Assert.ThrowsAsync<InvalidDataException>(
            () => dataLoader.GetDistilleryData(filePath));
        
        Assert.Equal($"'{filePath}' is found, but contains invalid data, when loading distillery data.", exception.Message);
    }
    
    [Fact]
    public async Task When_GetDistilleryData_WithNoDistilleryRecords_ExpectEmptyListOfDistilleryData()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/no-distillery-data.json");

        var dataLoader = new DataLoader();
        var distilleries = await dataLoader.GetDistilleryData(filePath);

        Assert.Empty(distilleries);
    }
    
    [Fact]
    public async Task When_GetDistilleryData_WithOneDistillery_ExpectListOfJustThatDistillery()
    {
        var expectedDistillery = new DistilleryData
        {
            Distillery = "Aberargie",
            Location = "Aberargie",
            Region = "Lowland",
            Founded = 2017,
            Owner = "Perth Distilling Co",
            DistilleryType = "Malt",
            Active = true
        };
            
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/single-distillery.json");
        
        var dataLoader = new DataLoader();
        var distilleries = await dataLoader.GetDistilleryData(filePath);

        var distillery = Assert.Single(distilleries);
        Assert.Equal(expectedDistillery, distillery);
    }
    
    [Fact]
    public async Task When_GetDistilleryData_WithThreeDistilleries_ExpectListOfJustThoseDistilleries()
    {
        var expectedDistilleries = new []
        {
            new DistilleryData
            {
                Distillery = "Aberargie",
                Location = "Aberargie",
                Region = "Lowland",
                Founded = 2017,
                Owner = "Perth Distilling Co",
                DistilleryType = "Malt",
                Active = true
            },
            new DistilleryData
            {
                Distillery = "Aberfeldy",
                Location = "Aberfeldy",
                Region = "Highland",
                Founded = 1896,
                Owner = "John Dewar & Sons",
                DistilleryType = "Malt",
                Active = true
            },
            new DistilleryData
            {
                Distillery = "Aberlour",
                Location = "Aberlour",
                Region = "Speyside",
                Founded = 1879,
                Owner = "Chivas Brothers",
                DistilleryType = "Malt",
                Active = true
            }
        };
        
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/three-distilleries.json");
        
        var dataLoader = new DataLoader();
        var distilleries = await dataLoader.GetDistilleryData(filePath);

        Assert.Equal(3, expectedDistilleries.Length);
        Assert.All(distilleries, distillery => Assert.Contains(distillery, expectedDistilleries));
    }
}