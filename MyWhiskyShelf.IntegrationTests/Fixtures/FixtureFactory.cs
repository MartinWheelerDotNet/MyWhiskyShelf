using Aspire.Hosting;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

public static class FixtureFactory
{
    public static readonly string[] DefaultTestingArguments =
    [
        "--no-launch-profile",
        "MYWHISKYSHELF_RUN_MIGRATIONS=true",
        "MYWHISKYSHELF_DATA_SEEDING_ENABLED=false",
        "MYWHISKYSHELF_UI_ENABLED=false",
        "MYWHISKYSHELF_PG_WEB_ENABLED=false",
        "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
    ];
    
    public static async Task<DistributedApplication> StartAsync(string[] args)
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>(args);

        var app = await builder.BuildAsync();
        await app.StartAsync();

        await app.Services.GetRequiredService<ResourceNotificationService>()
            .WaitForResourceAsync("WebApi", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(60));

        return app;
    }
}