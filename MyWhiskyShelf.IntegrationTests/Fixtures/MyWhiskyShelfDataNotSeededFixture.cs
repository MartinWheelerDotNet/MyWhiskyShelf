using JetBrains.Annotations;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfDataNotSeededFixture : MyWhiskyShelfFixture
{
    protected override async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MyWhiskyShelf_AppHost>(
        [
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=false",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false",
            "MYWHISKYSHELF_RUN_MIGRATIONS=false"
        ]);

        appHost.Services
            .ConfigureHttpClientDefaults(clientBuilder => clientBuilder.AddStandardResilienceHandler());

        return appHost;
    }
}