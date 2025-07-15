using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers.Data;
using DistilleryTestData = MyWhiskyShelf.TestHelpers.Data.DistilleryTestData;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

public static class WebApiTests
{
    private const string WebApiResourceName = "WebApi";

    [Collection("AspireTests")]
    public class WebApiSeededDataTests(MyWhiskyShelfBaseFixtureSeededDb fixture)
        : IClassFixture<MyWhiskyShelfBaseFixtureSeededDb>
    {
        [Fact]
        public async Task When_RequestingAllDistilleryNames_Expect_AllTestDistilleriesNamesToBeReturned()
        {
            const string endpoint = "/distilleries/names";
            List<string> expectedDistilleryNames =
            [
                DistilleryTestData.Aberargie.DistilleryName,
                DistilleryTestData.Aberfeldy.DistilleryName,
                DistilleryTestData.Aberlour.DistilleryName
            ];

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleryNames = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.All(expectedDistilleryNames, distillery => Assert.Contains(distillery, distilleryNames!)));
        }

        [Fact]
        public async Task When_RequestingAllDistilleries_Expect_AllDistilleriesReturned()
        {
            const string endpoint = "/distilleries";
            List<Distillery> expectedDistilleries =
            [
                DistilleryTestData.Aberargie,
                DistilleryTestData.Aberfeldy,
                DistilleryTestData.Aberlour
            ];

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleries = await response.Content.ReadFromJsonAsync<List<Distillery>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.All(expectedDistilleries, distillery => Assert.Contains(distillery, distilleries!)));
        }

        [Fact]
        public async Task When_RequestingDistilleryByName_Expect_CorrectDistilleryReturned()
        {
            var endpoint = $"/distilleries/{DistilleryTestData.Aberfeldy.DistilleryName}";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distillery = await response.Content.ReadFromJsonAsync<Distillery>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal(DistilleryTestData.Aberfeldy, distillery));
        }

        [Fact]
        public async Task When_SearchingAndNoMatchesFound_Expect_EmptyList()
        {
            const string endpoint = "/distilleries/name/search?pattern=anything";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleryNames = await response.Content.ReadFromJsonAsync<List<string>>();


            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Empty(distilleryNames!));
        }

        [Fact]
        public async Task When_SearchingExactMatchFound_Expect_ListWithJustThatDistilleryName()
        {
            var endpoint = $"/distilleries/name/search?pattern={DistilleryTestData.Aberfeldy.DistilleryName}";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleryNames = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal([DistilleryTestData.Aberfeldy.DistilleryName], distilleryNames));
        }

        [Fact]
        public async Task When_SearchingAndPatternIsNotProvided_Expect_BadRequestWithProblemDetails()
        {
            const string endpoint = "/distilleries/name/search";

            var expectedProblem = new ProblemDetails
            {
                Type = "urn:mywhiskyshelf:errors:missing-or-invalid-query-pattern",
                Title = "Missing or invalid query parameter",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Query parameter 'pattern' is required and cannot be empty.",
                Instance = endpoint
            };

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var problemResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode),
                () => Assert.Equivalent(expectedProblem, problemResponse));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("    ")]
        [InlineData("\t \n")]
        public async Task When_SearchingAndPatternIsEmptyOrWhiteSpace_Expect_BadRequestProblemResponse(
            string queryPattern)
        {
            var endpoint = $"/distilleries/name/search?pattern={queryPattern}";

            var expectedProblem = new ProblemDetails
            {
                Type = "urn:mywhiskyshelf:errors:missing-or-invalid-query-pattern",
                Title = "Missing or invalid query parameter",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Query parameter 'pattern' is required and cannot be empty.",
                Instance = "/distilleries/name/search"
            };

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var problemResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode),
                () => Assert.Equivalent(expectedProblem, problemResponse));
        }

        [Fact]
        public async Task When_AddingDistilleryAndDistilleryDoesNotExist_Expect_CreatedWithLocationHeaderSet()
        {
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.PostAsJsonAsync(
                "/distilleries/add",
                DistilleryTestData.Aberargie with { DistilleryName = "NewDistillery" });
            await httpClient.DeleteAsync("/distilleries/remove/NewDistillery");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("/distilleries/NewDistillery", response.Headers.Location!.ToString());
        }

        [Fact]
        public async Task When_RemovingDistilleryAndDistilleryExists_Expect_OkResponse()
        {
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.DeleteAsync("/distilleries/remove/Aberargie");

            await httpClient.PostAsJsonAsync("/distilleries/add/", DistilleryTestData.Aberargie);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task When_RemovingDistilleryAndDistilleryDoesNotExist_Expect_NotFoundProblemResponse()
        {
            var expectedProblem = new ProblemDetails
            {
                Type = "urn:mywhiskyshelf:errors:distillery-does-not-exist",
                Title = "Distillery does not exist.",
                Status = StatusCodes.Status404NotFound,
                Detail = "Cannot remove distillery 'FakeDistillery' as it does not exist.",
                Instance = "/distilleries/remove/FakeDistillery"
            };

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.DeleteAsync("/distilleries/remove/FakeDistillery");
            var problemResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.NotFound, response.StatusCode),
                () => Assert.Equivalent(expectedProblem, problemResponse));
        }

        [Fact]
        public async Task
            When_AddingDistilleryAndNameContainsSpecialCharacters_Expect_CreatedWithUrlEncodedLocationHeaderSet()
        {
            const string endpoint = "/distilleries/add";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.PostAsJsonAsync(
                endpoint,
                DistilleryTestData.Aberargie with { DistilleryName = "Burn O'Bennie" });


            await httpClient.DeleteAsync("/distilleries/remove/Burn%20O%27Bennie");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("/distilleries/Burn%20O%27Bennie", response.Headers.Location!.ToString());
        }

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
        public async Task When_RequestingAllDistilleries_Expect_EmptyListIsReturned()
        {
            const string endpoint = "/distilleries";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleries = await response.Content.ReadFromJsonAsync<List<Distillery>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal([], distilleries));
        }

        [Fact]
        public async Task When_RequestingDistilleryByName_Expect_NotFoundResponse()
        {
            var endpoint = $"/distilleries/{DistilleryTestData.Aberfeldy.DistilleryName}";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task When_Searching_Expect_EmptyListIsReturned()
        {
            const string endpoint = "/distilleries/name/search?pattern=anything";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleryNames = await response.Content.ReadFromJsonAsync<List<string>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Empty(distilleryNames!));
        }
    }
}