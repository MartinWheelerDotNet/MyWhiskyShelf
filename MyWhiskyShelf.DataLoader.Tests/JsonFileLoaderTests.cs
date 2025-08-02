using Microsoft.Extensions.Logging.Abstractions;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.DataLoader.Tests;

public class JsonFileLoaderTests
{
    [Fact]
    public async Task When_GetDistilleriesFromJsonWithFileNotFound_Expect_ExceptionThrown()
    {
        const string filename = "./not-a-valid-file.json";

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var exception =
            await Assert.ThrowsAsync<FileNotFoundException>(() => dataLoader.GetDistilleriesFromJsonAsync(filename));

        Assert.Equal($"'{filename}' not found when loading distillery data.", exception.Message);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithEmptyFileProvided_Expect_ExceptionThrown()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/empty-file.json");

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var exception =
            await Assert.ThrowsAsync<InvalidDataException>(() => dataLoader.GetDistilleriesFromJsonAsync(filePath));

        Assert.Equal($"'{filePath}' is found, but empty, when loading distillery data.", exception.Message);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithInvalidFormatProvided_Expect_ExceptionThrown()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/invalid-format.json");

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var exception =
            await Assert.ThrowsAsync<InvalidDataException>(() => dataLoader.GetDistilleriesFromJsonAsync(filePath));

        Assert.Equal($"'{filePath}' is found, but contains invalid data, when loading distillery data.",
            exception.Message);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithNoDistilleryRecords_Expect_EmptyListOfDistilleryData()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/no-distillery-data.json");

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync(filePath);

        Assert.Empty(distilleries);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithOneDistillery_Expect_ListOfJustThatDistillery()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/single-distillery.json");

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync(filePath);

        var distillery = Assert.Single(distilleries);
        Assert.Equal(DistilleryRequestTestData.Aberargie, distillery);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithThreeDistilleries_Expect_ListOfJustThoseDistilleries()
    {
        List<DistilleryRequest> expectedDistilleries =
        [
            DistilleryRequestTestData.Aberargie,
            DistilleryRequestTestData.Aberfeldy,
            DistilleryRequestTestData.Aberlour
        ];

        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/DistilleryData/three-distilleries.json");

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync(filePath);

        Assert.Equal(3, distilleries.Count);
        Assert.Equal(expectedDistilleries, distilleries);
    }
}