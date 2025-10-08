using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.WebApi.Contracts.Common;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

[Collection("DataSeeding")]
public class DataSeedingEnvironmentVariablesTest
{
    private static string[] GetArgs(bool? enableDataSeeding) {
        List<string> args = [
            "--no-launch-profile",
            "MYWHISKYSHELF_UI_ENABLED=false",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false",
            "MYWHISKYSHELF_RUN_MIGRATIONS=true"
          
        ];
        if (enableDataSeeding.HasValue) {
            args.Add($"MYWHISKYSHELF_DATA_SEEDING_ENABLED={enableDataSeeding}");
        }
        return args.ToArray();
    } 
    
    [Fact]
    public async Task When_AppHostDataSeedingIsTrue_Expect_DistilleryEntriesReturned()
    {
        await using var application = await FixtureFactory.StartAsync(GetArgs(true));
        using var httpClient = await application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries");
        var distilleries = await response.Content.ReadFromJsonAsync<PagedResponse<DistilleryResponse>>();

        Assert.True(distilleries!.Items.Count > 0);
    }
    
    [Fact]
    public async Task When_AppHostDataSeedingNotSet_Expect_DistilleryEntriesReturned()
    {
        await using var application = await FixtureFactory.StartAsync(GetArgs(null));
        using var httpClient = await application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries");
        var distilleries = await response.Content.ReadFromJsonAsync<PagedResponse<DistilleryResponse>>();

        Assert.True(distilleries!.Items.Count > 0);
    }
    
    [Fact]
    public async Task When_AppHostDataSeedingIsFalse_Expect_NoDistilleryEntriesReturned()
    {
        await using var application = await FixtureFactory.StartAsync(GetArgs(false));
        using var httpClient = await application.CreateAdminHttpsClientAsync();

        var response = await httpClient.GetAsync("/distilleries");
        var distilleries = await response.Content.ReadFromJsonAsync<PagedResponse<DistilleryResponse>>();

        Assert.Empty(distilleries!.Items);
    }
}