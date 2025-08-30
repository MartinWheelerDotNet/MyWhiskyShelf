using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

[Collection("DataSeededTestCollection")]
public class AppHostTestsMywhiskyshelfDataSeedingEnabled(MyWhiskyShelfDataSeededFixture fixture)
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

[Collection("AspireTests")]
public class AppHostTestsMywhiskyshelfDataSeedingDisabled(MyWhiskyShelfFixture fixture)
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_AppHostDataSeedingEnvironmentVariableIsFalse_Expect_NoDistilleryEntitiesReturned()
    {
        const string endpoint = "/distilleries";

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.GetAsync(endpoint);
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.NotNull(distilleries);
        Assert.True(distilleries.Count is 0);
    }
}