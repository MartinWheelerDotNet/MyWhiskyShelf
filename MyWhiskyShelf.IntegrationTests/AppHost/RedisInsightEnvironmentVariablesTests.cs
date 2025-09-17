using Aspire.Hosting.Redis;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

public class RedisInsightEnvironmentVariablesTests
{
    [Fact]
    public async Task When_RedisInsightEnvironmentVariableIsNotPresent_Expect_RedisInsightContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development"
            ]);

        Assert.NotEmpty(appHostBuilder.Resources.OfType<RedisInsightResource>());
    }

    
    [Fact]
    public async Task When_RedisInsightIsEnabledInDevelopment_Expect_RedisInsightContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development",
                "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=true"
            ]);

        Assert.NotEmpty(appHostBuilder.Resources.OfType<RedisInsightResource>());
    }

    [Fact]
    public async Task When_RedisInsightIsDisabledInDevelopment_Expect_RedisInsightContainerIsNotPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development",
                "MYWHISKYSHELF_REDIS_INSIGHT_ENABLED=false"
            ]);

        Assert.Empty(appHostBuilder.Resources.OfType<RedisInsightResource>());
    }

    [Fact]
    public async Task When_RedisInsightIsEnabledInProduction_Expect_RedisInsightContainerIsNotPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Production",
                "MYWHISKYSHELF_PG_WEB_ENABLED=true"
            ]);

        Assert.Empty(appHostBuilder.Resources.OfType<RedisInsightResource>());
    }
}