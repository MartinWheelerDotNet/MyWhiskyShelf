using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.StatusTests;

public class ServiceStatusTests(MyWhiskyShelfFixture myWhiskyShelfFixture) : IClassFixture<MyWhiskyShelfFixture>
{
    [Theory]
    [InlineData("WebApi")]
    public async Task When_ResourceIsRunning_Expect_ResourceHealthEndpointReturnsHealthy(string resourceName)
    {
        const string endpointName = "/health";
        using var httpClient = myWhiskyShelfFixture.Application.CreateHttpClient(resourceName);
        
        var response = await httpClient.GetAsync(endpointName);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Equal("Healthy", body));
    }
}