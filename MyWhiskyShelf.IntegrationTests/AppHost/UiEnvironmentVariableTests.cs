using Projects;

namespace MyWhiskyShelf.IntegrationTests.AppHost;

public class UiEnvironmentVariableTests
{
    [Fact]
    public async Task When_UiEnvironmentVariableIsNotProvided_Expect_UiContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>();

        Assert.Contains(appHostBuilder.Resources, r => r.Name == "UI");
    }

    [Fact]
    public async Task When_UiIsEnabled_Expect_UiContainerIsPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>(["MYWHISKYSHELF_UI_ENABLED=true"]);

        Assert.Contains(appHostBuilder.Resources, r => r.Name == "UI");
    }

    [Fact]
    public async Task When_RunMigrationsIsDisabledInDevelopment_Expect_MigrationsContainerIsNotPresent()
    {
        await using var appHostBuilder = await DistributedApplicationTestingBuilder
            .CreateAsync<MyWhiskyShelf_AppHost>(["MYWHISKYSHELF_UI_ENABLED=false"]);

        Assert.DoesNotContain(appHostBuilder.Resources, r => r.Name == "UI");
    }
}