using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers.Data;
using Xunit.Sdk;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiWhiskyBottleTests(MyWhiskyShelfBaseFixtureEmptyDb fixture)
    : IClassFixture<MyWhiskyShelfBaseFixtureEmptyDb>
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_AddWhiskyBottle_Expect_WhiskyBottleIsCreatedWithLocationHeaderSet()
    {
        const string endpoint = "/whisky-bottle/add";

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var postResponse = await httpClient.PostAsJsonAsync(endpoint, WhiskyBottleRequestTestData.AllValuesPopulated);
        var parts = postResponse.Headers.Location!.OriginalString.Trim('/').Split("/");

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode),
            () => Assert.Equal("whisky-bottle", parts[0]),
            () => AssertIsGuidAndNotEmpty(parts[1]));
    }

    [Fact]
    public async Task When_AddWhiskyBottleAndBottleCannotBeAddedToDatabase_Expect_ValidationProblemDetails()
    {
        const string endpoint = "/whisky-bottle/add";

        // we are deliberately breaking the model constraints here to so that the database will attempt to insert
        // an invalid entity, which will cause it to fail.
        var whiskyBottleWithoutName = WhiskyBottleRequestTestData.AllValuesPopulated with { Name = null! };

        var expectedValidationProblem = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Type = "urn:mywhiskyshelf:validation-errors:whisky-bottle",
            Status = 400,
            Errors = new Dictionary<string, string[]>
            {
                ["WhiskyBottleRequest"] = ["An error occurred trying to add the whisky bottle to the database."]
            }
        };

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.PostAsJsonAsync(endpoint, whiskyBottleWithoutName);
        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, body);
    }

    private static void AssertIsGuidAndNotEmpty(string guidString)
    {
        if (!Guid.TryParse(guidString, out var result))
            throw new XunitException($"Expected a valid GUID but got: '{guidString}'");

        if (Guid.Empty.Equals(result))
            throw new XunitException($"Expected a none-empty GUID but got: '{result}'");
    }
}