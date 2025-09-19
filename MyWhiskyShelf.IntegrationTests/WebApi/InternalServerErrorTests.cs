using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(BrokenFixture))]
public class InternalServerErrorTests(BrokenFixture fixture)
{
    [Theory]
    [InlineData("/distilleries")]
    public async Task When_GettingAndAnErrorOccurs_Expect_ProblemReturned(string endpoint)
    {
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        
        var response = await httpClient.GetAsync(endpoint);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => Assert.Equal("urn:mywhiskyshelf:errors:distilleries-get-all-failed", problemDetails.Type),
            () => Assert.Equal("Failed to get-all distilleries", problemDetails.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status),
            () => Assert.Equal("/distilleries", problemDetails.Instance),
            () => Assert.Contains("An unexpected error occurred", problemDetails.Detail));
    }
}