using Projects;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

public class RunMigrationsEnvironmentVariableTests
{
    [Fact]
    public async Task When_RunMigrationsEnvironmentVariableIsNotProvided_Expect_MigrationsContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development"
            ]);

        Assert.Contains(appHostBuilder.Resources, r => r.Name == "migrations");
    }
    
    [Fact]
    public async Task When_RunMigrationsIsEnabledInDevelopment_Expect_MigrationsContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development",
                "MYWHISKYSHELF_RUN_MIGRATIONS=true"
            ]);

        Assert.Contains(appHostBuilder.Resources, r => r.Name == "migrations");
    }

    [Fact]
    public async Task When_RunMigrationsIsDisabledInDevelopment_Expect_MigrationsContainerIsNotPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>([
                "--environment=Development",
                "MYWHISKYSHELF_RUN_MIGRATIONS=false"
            ]);

        Assert.DoesNotContain(appHostBuilder.Resources, r => r.Name == "migrations");
    }
}