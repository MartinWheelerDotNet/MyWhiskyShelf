using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiWhiskyBottleTests(MyWhiskyShelfBaseFixtureEmptyDb fixture)
    : IClassFixture<MyWhiskyShelfBaseFixtureEmptyDb>
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_AddWhiskyBottle_Expect_CreatedWithLocationHeaderSet()
    {
        const string endpoint = "/whiskyBottle/add";

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.PostAsJsonAsync(endpoint, WhiskyBottleTestData.AllValuesPopulated);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode),
            () => Assert.Equal("/whiskyBottle/All%20Values%20Populated", response.Headers.Location!.ToString()));
    }

    [Fact]
    public async Task When_AddWhiskyBottleAndBottleCannotBeAddedToDatabase_Expect_ValidationProblemDetails()
    {
        const string endpoint = "/whiskyBottle/add";

        // we are deliberately breaking the model constraints here to so that the database will attempt to insert
        // an invalid entity, which will cause it to fail.
        var whiskyBottleWithoutName = WhiskyBottleTestData.AllValuesPopulated with { Name = null! };

        var expectedValidationProblem = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Instance = null,
            Errors = new Dictionary<string, string[]>
            {
                ["WhiskyBottle"] =
                [
                    """
                    An error occurred trying to add the whisky bottle to the database.
                    Ensure all required fields have been set.
                    """
                ]
            }
        };

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.PostAsJsonAsync(endpoint, whiskyBottleWithoutName);
        var body = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, body);
    }
}