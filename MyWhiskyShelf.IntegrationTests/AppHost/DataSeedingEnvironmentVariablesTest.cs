using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

[Collection("DataSeeding")]
public class DataSeedingEnvironmentVariablesTest
{
    [Fact]
    public async Task When_AppHostDataSeedingIsTrue_Expect_DistilleryEntriesReturned()
    {
        var args = new[]
        {
            "--no-launch-profile",
            "MYWHISKYSHELF_RUN_MIGRATIONS=true",
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=true",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
        };

        await using var application = await FixtureFactory.StartAsync(args);
        using var httpClient = await application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries");
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.True(distilleries!.Count > 0);
    }
    
    [Fact]
    public async Task When_AppHostDataSeedingNotSet_Expect_DistilleryEntriesReturned()
    {
        var args = new[]
        {
            "--no-launch-profile",
            "MYWHISKYSHELF_RUN_MIGRATIONS=true",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
        };

        await using var application = await FixtureFactory.StartAsync(args);
        using var httpClient = await application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries");
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.True(distilleries!.Count > 0);
    }
    
    [Fact]
    public async Task When_AppHostDataSeedingIsFalse_Expect_NoDistilleryEntriesReturned()
    {
        var args = new[]
        {
            "--no-launch-profile",
            "MYWHISKYSHELF_RUN_MIGRATIONS=true",
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=false",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
        };

        await using var application = await FixtureFactory.StartAsync(args);
        using var httpClient = await application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries");
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.Empty(distilleries!);
    }
}