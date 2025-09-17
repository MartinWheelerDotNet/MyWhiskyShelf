using Aspire.Hosting;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

public sealed class MyWhiskyShelfDataSeedingFixtureFactory
{
    public async Task<DistributedApplication> StartAsync(params string[] args)
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