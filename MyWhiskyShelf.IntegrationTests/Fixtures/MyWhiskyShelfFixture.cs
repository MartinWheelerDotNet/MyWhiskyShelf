using Aspire.Hosting;
using JetBrains.Annotations;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfFixture : IAsyncLifetime
{
    public DistributedApplication Application { get; private set; } = null!;

    public virtual async Task InitializeAsync()
    {
        var appHost = await CreateDefaultAppHost();
        Application = await appHost.BuildAsync();
        await Application.StartAsync();

        await WaitForRunningState(Application, "WebApi");
    }

    public virtual async Task DisposeAsync()
    {
        await Application.DisposeAsync();
    }

    private static async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MyWhiskyShelf_AppHost>(
            ["MYWHISKYSHELF_DATA_SEEDING_ENABLED=false"]);

        appHost.Services
            .ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        return appHost;
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
}