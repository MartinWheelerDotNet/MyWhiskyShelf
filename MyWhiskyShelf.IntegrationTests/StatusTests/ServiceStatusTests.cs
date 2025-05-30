using FluentAssertions;
using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.StatusTests;

public class ServiceStatusTests(MyWhiskyShelfFixture myWhiskyShelfFixture) : IClassFixture<MyWhiskyShelfFixture>
{
    [Theory]
    [InlineData("WebApi")]
    public async Task ResourceHealthEndpointReturnsHealthyWhenResourceIsRunning(string resourceName)
    {
        const string endpointName = "/health";
        using var httpClient = myWhiskyShelfFixture.Application.CreateHttpClient(resourceName);
        
        var response = await httpClient.GetAsync(endpointName);
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Multiple(
            () => response.StatusCode.Should().Be(HttpStatusCode.OK),
            () => body.Should().Be("Healthy"));
    }
}