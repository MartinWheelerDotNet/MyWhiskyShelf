using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiDistilleryNameTests(MyWhiskyShelfFixture fixture) : IAsyncLifetime
{
    private const string WebApiResourceName = "WebApi";

    public async Task InitializeAsync()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        await DatabaseSeeding.SeedDatabase(httpClient);
    }
    
    [Fact]
    public async Task When_GettingAllDistilleryNamesDetails_Expect_AllDistilleriesNameDetailsToBeReturned()
    {
        List<string> expectedDistilleryNames =
        [
            DistilleryResponseTestData.Aberargie.Name,
            DistilleryResponseTestData.Aberfeldy.Name,
            DistilleryResponseTestData.Aberlour.Name
        ];
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.GetAsync("/distilleries/names");
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.All(distilleryNames!, details =>
            {
                var (distilleryName, id) = details;
                Assert.Contains(distilleryName, expectedDistilleryNames);
                Assert.NotEqual(Guid.Empty, id);
            }));
    }

    [Fact]
    public async Task When_SearchingByNameAndNoMatchesFound_Expect_EmptyList()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.GetAsync("/distilleries/name/search?pattern=anything");
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();


        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Empty(distilleryNames!));
    }

    [Fact]
    public async Task When_SearchingByNameAndMatchesExactly_Expect_ListWithJustThoseDistilleryNameDetails()
    {
        var endpoint = $"/distilleries/name/search?pattern={DistilleryRequestTestData.Aberfeldy.Name}";
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.GetAsync(endpoint);
        var distilleryNameDetails = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Single(distilleryNameDetails!),
            () => Assert.Equal(
                DistilleryRequestTestData.Aberfeldy.Name,
                distilleryNameDetails![0].Name),
            () => Assert.IsType<Guid>(distilleryNameDetails![0].Id));
    }

    [Fact]
    public async Task When_SearchingByNameAndFuzzyMatches_Expect_ListWithJustThoseFuzzyMatchedNameDetails()
    {
        const string endpoint = "/distilleries/name/search?pattern=erl";
        List<string> expectedDistilleryNames =
        [
            DistilleryResponseTestData.Aberfeldy.Name,
            DistilleryResponseTestData.Aberlour.Name
        ];
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        
        var response = await httpClient.GetAsync(endpoint);
        var distilleryNameDetails = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();
        
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.All(expectedDistilleryNames, expectedDistilleryName
                => Assert.Contains(distilleryNameDetails!, actual => expectedDistilleryName == actual.Name)),
            () => Assert.All(distilleryNameDetails!, distilleryNameDetail
                => Assert.NotEqual(Guid.Empty, distilleryNameDetail.Id)));
    }

    [Fact]
    public async Task When_SearchingByNameAndPatternIsNotProvided_Expect_BadRequestWithValidationProblemDetails()
    {
        const string endpoint = "/distilleries/name/search";

        var expectedProblem = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Missing or empty query parameters",
            Type = "urn:mywhiskyshelf:validation-errors:query-parameter",
            Errors = new Dictionary<string, string[]>
            {
                { "pattern", ["Query parameter 'pattern' is required and cannot be empty."] }
            }
        };

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.GetAsync(endpoint);
        var problemResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode),
            () => Assert.Equivalent(expectedProblem, problemResponse));
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \n")]
    public async Task When_SearchingByNameAndPatternIsEmptyOrWhiteSpace_Expect_BadRequestProblemResponse(
        string pattern)
    {
        var endpoint = $"/distilleries/name/search?pattern={pattern}";

        var expectedProblem = new ValidationProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Missing or empty query parameters",
            Type = "urn:mywhiskyshelf:validation-errors:query-parameter",
            Errors = new Dictionary<string, string[]>
            {
                { "pattern", ["Query parameter 'pattern' is required and cannot be empty."] }
            }
        };

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.GetAsync(endpoint);
        var problemResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode),
            () => Assert.Equivalent(expectedProblem, problemResponse));
    }

    public async Task DisposeAsync()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        await DatabaseSeeding.ClearDatabase(httpClient);
    }
}