using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

public static class WebApiDistilleryNameTests
{
    private const string WebApiResourceName = "WebApi";

    [Collection("AspireTests")]
    public class WebApiSeededDataTests(MyWhiskyShelfBaseFixtureSeededDb fixture)
        : IClassFixture<MyWhiskyShelfBaseFixtureSeededDb>
    {
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
            var response = await httpClient.GetAsync(endpoint);
            var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.All(distilleryNames!, details =>
                {
                    var (distilleryName, identifier) = details;
                    Assert.Contains(distilleryName, expectedDistilleryNames);
                    Assert.NotEqual(Guid.Empty, identifier);
                }));
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
            var response = await httpClient.GetAsync(endpoint);
            var distilleryNameDetails = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Single(distilleryNameDetails!),
                () => Assert.Equal(DistilleryRequestTestData.Aberfeldy.DistilleryName,
                    distilleryNameDetails!.First().DistilleryName),
                () => Assert.IsType<Guid>(distilleryNameDetails!.First().Identifier));
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

    [Collection("AspireTests")]
    public class WebApiNotSeededDataTests(MyWhiskyShelfBaseFixtureEmptyDb fixture)
        : IClassFixture<MyWhiskyShelfBaseFixtureEmptyDb>
    {
        [Fact]
        public async Task When_RequestingAllDistilleryNames_Expect_EmptyListIsReturned()
        {
            const string endpointName = "/distilleries/names";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpointName);
            var distilleries = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal([], distilleries));
        }

        [Fact]
        public async Task When_Searching_Expect_EmptyListIsReturned()
        {
            const string endpoint = "/distilleries/name/search?pattern=aber";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleryNames = await response.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Empty(distilleryNames!));
        }
    }
}