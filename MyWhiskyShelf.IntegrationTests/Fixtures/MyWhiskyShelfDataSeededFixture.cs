using JetBrains.Annotations;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.Fixtures;

[UsedImplicitly]
public class MyWhiskyShelfDataSeededFixture : MyWhiskyShelfFixture
{
    protected override async Task<IDistributedApplicationTestingBuilder> CreateDefaultAppHost()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MyWhiskyShelf_AppHost>(
        [
            "MYWHISKYSHELF_DATA_SEEDING_ENABLED=true",
            "MYWHISKYSHELF_RUN_MIGRATIONS=true",
            "MYWHISKYSHELF_PG_WEB_ENABLED=false",
            "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
        ]);

        return appHost;
    }
}