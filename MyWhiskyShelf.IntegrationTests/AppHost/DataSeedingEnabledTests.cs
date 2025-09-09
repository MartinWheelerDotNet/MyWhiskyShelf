using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

[Collection("DataSeededTestCollection")]
public class DataSeedingEnabledTests(MyWhiskyShelfDataSeededFixture fixture)
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_AppHostDataSeedingEnvironmentVariableIsTrue_Expect_DistilleryEntriesReturned()
    {
        const string endpoint = "/distilleries";

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.GetAsync(endpoint);
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.NotNull(distilleries);
        Assert.True(distilleries.Count > 0);
    }
}