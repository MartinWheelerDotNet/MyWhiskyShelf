using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

public static class WebApiDistilleriesTests
{
    private const string WebApiResourceName = "WebApi";

    private static bool EqualsIgnoringId(DistilleryResponse expected, DistilleryResponse actual)
    {
        return expected with { Id = Guid.Empty } == actual with { Id = Guid.Empty };
    }

    [Collection("AspireTests")]
    public class WebApiSeededDataTests(MyWhiskyShelfBaseFixtureSeededDb fixture)
        : IClassFixture<MyWhiskyShelfBaseFixtureSeededDb>
    {
        [Fact]
        public async Task When_RequestingAllDistilleries_Expect_AllDistilleriesReturned()
        {
            const string endpoint = "/distilleries";
            List<DistilleryResponse> expectedDistilleries =
            [
                DistilleryResponseTestData.Aberargie,
                DistilleryResponseTestData.Aberfeldy,
                DistilleryResponseTestData.Aberlour
            ];

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.All(expectedDistilleries, distillery
                    => Assert.Contains(distilleries!, actual => EqualsIgnoringId(distillery, actual))),
                () => Assert.All(distilleries!, distillery
                    => Assert.NotEqual(Guid.Empty, distillery.Id)));
        }

        [Fact]
        public async Task When_RequestingDistilleryByName_Expect_CorrectDistilleryReturned()
        {
            const string endpoint = "/distilleries/Aberfeldy";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distillery = await response.Content.ReadFromJsonAsync<DistilleryResponse>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.True(EqualsIgnoringId(DistilleryResponseTestData.Aberfeldy, distillery!)));
        }

        [Fact]
        public async Task When_AddingDistilleryAndDistilleryDoesNotExist_Expect_CreatedWithLocationHeaderSet()
        {
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.PostAsJsonAsync(
                "/distilleries",
                DistilleryRequestTestData.Aberargie with { DistilleryName = "NewDistillery" });
            await httpClient.DeleteAsync("/distilleries/remove/NewDistillery");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("/distilleries/NewDistillery", response.Headers.Location!.ToString());
        }

        [Fact]
        public async Task When_RemovingDistilleryAndDistilleryExists_Expect_OkResponse()
        {
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            
            await httpClient.PostAsJsonAsync(
                "/distilleries",
                DistilleryRequestTestData.Aberargie with { DistilleryName = "NewDistilleryToRemove" });
            
            var response = await httpClient.DeleteAsync("/distilleries/remove/NewDistilleryToRemove");
            
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
            const string endpoint = "/distilleries";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.PostAsJsonAsync(
                endpoint,
                DistilleryRequestTestData.Aberargie with { DistilleryName = "Burn O'Bennie" });

            await httpClient.DeleteAsync("/distilleries/remove/Burn%20O%27Bennie");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("/distilleries/Burn%20O%27Bennie", response.Headers.Location!.ToString());
        }
    }

    [Collection("AspireTests")]
    public class WebApiNotSeededDataTests(MyWhiskyShelfBaseFixtureEmptyDb fixture)
        : IClassFixture<MyWhiskyShelfBaseFixtureEmptyDb>
    {
        [Fact]
        public async Task When_RequestingAllDistilleries_Expect_EmptyListIsReturned()
        {
            const string endpoint = "/distilleries";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);
            var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
                () => Assert.Equal([], distilleries));
        }

        [Fact]
        public async Task When_RequestingDistilleryByName_Expect_NotFoundResponse()
        {
            const string endpoint = "/distilleries/Aberfeldy";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}