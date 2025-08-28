using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Seeding;
using MyWhiskyShelf.Infrastructure.Tests.TestData;

namespace MyWhiskyShelf.Infrastructure.Tests.Seeding;

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
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/SeedingData/empty-file.json");

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var exception =
            await Assert.ThrowsAsync<InvalidDataException>(() => dataLoader.GetDistilleriesFromJsonAsync(filePath));

        Assert.Equal($"'{filePath}' is found, but empty, when loading distillery data.", exception.Message);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithInvalidFormatProvided_Expect_ExceptionThrown()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/SeedingData/invalid-format.json");

        var dataLoader = new JsonFileLoader(NullLogger<JsonFileLoader>.Instance);
        var exception =
            await Assert.ThrowsAsync<InvalidDataException>(() => dataLoader.GetDistilleriesFromJsonAsync(filePath));

        Assert.Equal($"'{filePath}' is found, but contains invalid data, when loading distillery data.",
            exception.Message);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithNoDistilleryRecords_Expect_EmptyListOfDistilleryData()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/SeedingData/no-distillery-data.json");

        var fakeLogger = new FakeLogger<JsonFileLoader>();
        var dataLoader = new JsonFileLoader(fakeLogger);
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync(filePath);

        Assert.Multiple(
            () => Assert.Empty(distilleries),
            () => Assert.Equal("0 distilleries loaded", fakeLogger.LatestRecord.Message));
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithANullList_Expect_EmptyListOfDistilleryData()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/SeedingData/null-file.json");

        var fakeLogger = new FakeLogger<JsonFileLoader>();
        var dataLoader = new JsonFileLoader(fakeLogger);
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync(filePath);

        Assert.Multiple(
            () => Assert.Empty(distilleries),
            () => Assert.Equal("0 distilleries loaded", fakeLogger.LatestRecord.Message));
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithOneDistillery_Expect_ListOfJustThatDistillery()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/SeedingData/single-distillery.json");
        var fakeLogger = new FakeLogger<JsonFileLoader>();
        
        var dataLoader = new JsonFileLoader(fakeLogger);
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync(filePath);

        var distillery = Assert.Single(distilleries);
        Assert.Equal(DistilleryTestData.Generic with { Name = "Single Distillery" }, distillery);
        Assert.Equal("1 distilleries loaded", fakeLogger.LatestRecord.Message);
    }

    [Fact]
    public async Task When_GetDistilleriesFromJsonWithThreeDistilleries_Expect_ListOfJustThoseDistilleries()
    {
        List<Distillery> expectedDistilleries =
        [
            DistilleryTestData.Generic with { Name = "Distillery A" },
            DistilleryTestData.Generic with { Name = "Distillery B" },
            DistilleryTestData.Generic with { Name = "Distillery C" }
        ];

        var filePath = Path.Combine(AppContext.BaseDirectory, "Resources/SeedingData/three-distilleries.json");
        var fakeLogger = new FakeLogger<JsonFileLoader>();
        
        var dataLoader = new JsonFileLoader(fakeLogger);
        var distilleries = await dataLoader.GetDistilleriesFromJsonAsync(filePath);

        Assert.Equal(3, distilleries.Count);
        Assert.Equal(expectedDistilleries, distilleries);
        Assert.Equal("3 distilleries loaded", fakeLogger.LatestRecord.Message);
    }
}