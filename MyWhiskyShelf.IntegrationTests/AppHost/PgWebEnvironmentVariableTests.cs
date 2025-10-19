using Aspire.Hosting.Postgres;
using Projects;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

public class PgWebEnvironmentVariableTests
{
    [Fact]
    public async Task When_PgWebEnvironmentVariableIsNotProvided_Expect_PgWebContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development"
            ]);

        Assert.NotEmpty(appHostBuilder.Resources.OfType<PgWebContainerResource>());
    }

    [Fact]
    public async Task When_PgWebIsEnabledInDevelopment_Expect_PgWebContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development",
                "MYWHISKYSHELF_PG_WEB_ENABLED=true"
            ]);

        Assert.NotEmpty(appHostBuilder.Resources.OfType<PgWebContainerResource>());
    }

    [Fact]
    public async Task When_PgWebIsDisabledInDevelopment_Expect_PgWebContainerIsNotPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development",
                "MYWHISKYSHELF_PG_WEB_ENABLED=false"
            ]);

        Assert.Empty(appHostBuilder.Resources.OfType<PgWebContainerResource>());
    }

    [Fact]
    public async Task When_PgWebIsEnabledInProduction_Expect_PgWebContainerIsNotPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Production",
                "MYWHISKYSHELF_PG_WEB_ENABLED=true"
            ]);

        Assert.Empty(appHostBuilder.Resources.OfType<PgWebContainerResource>());
    }
}