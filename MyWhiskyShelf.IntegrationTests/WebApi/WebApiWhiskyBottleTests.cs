using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers.Data;
using Xunit.Sdk;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiWhiskyBottleTests(MyWhiskyShelfFixture fixture)
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_AddWhiskyBottle_Expect_WhiskyBottleIsCreatedWithLocationHeaderSet()
    {
        const string endpoint = "/whisky-bottle";

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var postResponse = await httpClient.PostAsJsonAsync(endpoint, WhiskyBottleRequestTestData.AllValuesPopulated);
        await httpClient.DeleteAsync(postResponse.Headers.Location);
        var parts = postResponse.Headers.Location!.OriginalString.Trim('/').Split("/");

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode),
            () => Assert.Equal("whisky-bottle", parts[0]),
            () => AssertIsGuidAndNotEmpty(parts[1]));
    }

    [Fact]
    public async Task When_AddWhiskyBottleAndBottleCannotBeAddedToDatabase_Expect_ValidationProblemDetails()
    {
        const string endpoint = "/whisky-bottle";

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

    [Fact]
    public async Task When_DeleteWhiskyBottleAndBottleDoesNotExist_Expect_NotFoundProblemDetails()
    {
        var id = Guid.NewGuid();
        var endpoint = $"/whisky-bottle/{id}";
        var expectedProblem = new ProblemDetails
        {
            Type = "urn:mywhiskyshelf:errors:whisky-bottle-does-not-exist",
            Title = "whisky-bottle does not exist.",
            Status = StatusCodes.Status404NotFound,
            Detail = $"Cannot remove whisky-bottle '{id}' as it does not exist.",
            Instance = $"/whisky-bottle/{id}"
        };

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.DeleteAsync(endpoint);
        var problemResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.NotFound, response.StatusCode),
            () => Assert.Equivalent(expectedProblem, problemResponse));
    }

    [Fact]
    public async Task When_DeleteWhiskyBottleAndBottleDoesExist_Expect_OkResponse()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var createResponse = await httpClient.PostAsJsonAsync(
            "/whisky-bottle",
            WhiskyBottleRequestTestData.AllValuesPopulated);

        var response = await httpClient.DeleteAsync(createResponse.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdateWhiskyBottleAndWhiskyBottleIsFound_Expect_OkResponse()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var createResponse = await httpClient.PostAsJsonAsync(
            "/whisky-bottle", 
            WhiskyBottleRequestTestData.AllValuesPopulated);
        
        var response = await httpClient.PutAsJsonAsync(
            createResponse.Headers.Location,
            WhiskyBottleRequestTestData.AllValuesPopulated with { VolumeRemainingCl = 20 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    private static void AssertIsGuidAndNotEmpty(string guidString)
    {
        if (!Guid.TryParse(guidString, out var result))
            throw new XunitException($"Expected a valid GUID but got: '{guidString}'");

        if (Guid.Empty.Equals(result))
            throw new XunitException($"Expected a none-empty GUID but got: '{result}'");
    }
}