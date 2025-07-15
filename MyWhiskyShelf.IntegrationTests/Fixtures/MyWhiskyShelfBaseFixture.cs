using Aspire.Hosting;
using Microsoft.Extensions.Hosting;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

public class MyWhiskyShelfBaseFixture : IAsyncLifetime
{
    protected bool UseDataSeeding { get; init; }

    public DistributedApplication Application { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var appHost = await CreateDefaultAppHost();
        Application = await appHost.BuildAsync();
        Application.Start();

        await WaitForRunningState(Application, "WebApi");
    }

    public async Task DisposeAsync()
    {
        await Application.DisposeAsync();
    }

    private async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MyWhiskyShelf_AppHost>(
            [$"MYWHISKYSHELF_DATA_SEEDING_ENABLED={UseDataSeeding}"]);

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