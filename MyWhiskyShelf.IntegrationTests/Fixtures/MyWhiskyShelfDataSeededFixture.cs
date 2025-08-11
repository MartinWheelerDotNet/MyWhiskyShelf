using JetBrains.Annotations;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfDataSeededFixture : MyWhiskyShelfFixture
{
    protected override async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MyWhiskyShelf_AppHost>(
            ["MYWHISKYSHELF_DATA_SEEDING_ENABLED=true"]);

        appHost.Services
            .ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        return appHost;
    }
}