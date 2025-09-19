using Aspire.Hosting;
using JetBrains.Annotations;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class BrokenFixture : IAsyncLifetime
{
    public DistributedApplication Application { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MyWhiskyShelf_AppHost>(
        [
            "MYWHISKYSHELF_RUN_MIGRATIONS=false",
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=false",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
        ]);

        Application = await appHost.BuildAsync();
        await Application.StartAsync();
        
        await WaitForRunningState(Application, "WebApi");
    }
    
    private static async Task WaitForRunningState(
        DistributedApplication application,
        string serviceName,
        TimeSpan? timeout = null)
    {
        await application.Services.GetRequiredService<ResourceNotificationService>()
            .WaitForResourceAsync(serviceName, KnownResourceStates.Running)
            .WaitAsync(timeout ?? TimeSpan.FromSeconds(30));
    }

    public async Task DisposeAsync()
    {
        await Application.DisposeAsync();  
    } 
}