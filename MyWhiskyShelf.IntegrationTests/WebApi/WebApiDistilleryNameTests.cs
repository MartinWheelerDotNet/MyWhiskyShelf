using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiDistilleryNameTests(MyWhiskyShelfFixture fixture)
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_GettingAllDistilleryNamesDetails_Expect_AllDistilleriesNameDetailsToBeReturned()
    {
        const string endpoint = "/distilleries/names";

        List<string> expectedDistilleryNames =
        [
            DistilleryResponseTestData.Aberargie.DistilleryName,
            DistilleryResponseTestData.Aberfeldy.DistilleryName,
            DistilleryResponseTestData.Aberlour.DistilleryName
        ];

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var addedIds = await SeedTestData(httpClient);

        var response = await httpClient.GetAsync(endpoint);
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();
        await DatabaseSeeding.RemoveDistilleries(httpClient, addedIds);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.All(distilleryNames!, details =>
            {
                var (distilleryName, identifier) = details;
                Assert.Contains(distilleryName, expectedDistilleryNames);
                Assert.NotEqual(Guid.Empty, identifier);
            }));
    }

    private static async Task<List<Guid>> SeedTestData(HttpClient httpClient)
    {
        var addedIds = await DatabaseSeeding.AddDistilleries(
            httpClient,
            DistilleryRequestTestData.Aberargie,
            DistilleryRequestTestData.Aberfeldy,
            DistilleryRequestTestData.Aberlour);
        return addedIds;
    }

    [Fact]
    public async Task When_SearchingByNameAndNoMatchesFound_Expect_EmptyList()
    {
        const string endpoint = "/distilleries/name/search?pattern=anything";

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.GetAsync(endpoint);
        var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();


        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Empty(distilleryNames!));
    }

    [Fact]
    public async Task When_SearchingByNameAndMatchesExactly_Expect_ListWithJustThoseDistilleryNameDetails()
    {
        var endpoint = $"/distilleries/name/search?pattern={DistilleryRequestTestData.Aberfeldy.DistilleryName}";

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var addedIds = await SeedTestData(httpClient);

        var response = await httpClient.GetAsync(endpoint);
        var distilleryNameDetails = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();
        await DatabaseSeeding.RemoveDistilleries(httpClient, addedIds);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Single(distilleryNameDetails!),
            () => Assert.Equal(
                DistilleryRequestTestData.Aberfeldy.DistilleryName,
                distilleryNameDetails![0].DistilleryName),
            () => Assert.IsType<Guid>(distilleryNameDetails![0].Identifier));
    }

    [Fact]
    public async Task When_SearchingByNameAndFuzzyMatches_Expect_ListWithJustThoseFuzzyMatchedNameDetails()
    {
        const string endpoint = "/distilleries/name/search?pattern=erl";
        List<string> expectedDistilleryNames =
        [
            DistilleryResponseTestData.Aberfeldy.DistilleryName,
            DistilleryResponseTestData.Aberlour.DistilleryName
        ];

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var addedIds = await SeedTestData(httpClient);

        var response = await httpClient.GetAsync(endpoint);
        var distilleryNameDetails = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();
        await DatabaseSeeding.RemoveDistilleries(httpClient, addedIds);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.All(expectedDistilleryNames, expectedDistilleryName
                => Assert.Contains(distilleryNameDetails!, actual => expectedDistilleryName == actual.DistilleryName)),
            () => Assert.All(distilleryNameDetails!, distilleryNameDetail
                => Assert.NotEqual(Guid.Empty, distilleryNameDetail.Identifier)));
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
}