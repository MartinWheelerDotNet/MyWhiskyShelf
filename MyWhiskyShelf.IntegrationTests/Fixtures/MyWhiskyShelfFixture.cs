using Aspire.Hosting;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfFixture : IDisposable
{
    public readonly DistributedApplication Application;

    public MyWhiskyShelfFixture()
    { 
        var appHost = CreateDefaultAppHost().Result;
        Application = appHost.BuildAsync().Result;
        Application.Start();

        WaitForRunningState(Application, "WebApi")
            .ConfigureAwait(ConfigureAwaitOptions.None)
            .GetAwaiter()
            .GetResult();
    }
    
    private static async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyWhiskyShelf_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        
        return appHost;
    }

    private static async Task WaitForRunningState(
        DistributedApplication application,
        string serviceName,
        TimeSpan? timeout = null) 
        => await application.Services.GetRequiredService<ResourceNotificationService>()
            .WaitForResourceAsync(serviceName, KnownResourceStates.Running)
            .WaitAsync(timeout ?? TimeSpan.FromSeconds(30))
    ;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Application.Dispose();
    }
}