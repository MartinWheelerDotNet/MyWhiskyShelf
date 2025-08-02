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
    public class WebApiSeededDataTests(MyWhiskyShelfFixture fixture)
        : IClassFixture<MyWhiskyShelfFixture>
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
        public async Task When_RequestingDistilleryByIdAndDistilleryExists_Expect_CorrectDistilleryReturned()
        {
            const string detailsEndpoint = "/distilleries/name/search?pattern=Aberfeldy";
            
            // get the ID of an existing distillery.
            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var detailsResponse = await httpClient.GetAsync(detailsEndpoint);
            var distilleryDetails = await detailsResponse.Content.ReadFromJsonAsync<List<DistilleryNameDetails>>();
            
            var distilleryEndpoint = $"/distilleries/{distilleryDetails!.First().Identifier}";
            
            var distilleryResponse = await httpClient.GetAsync(distilleryEndpoint);
            var distillery = await distilleryResponse.Content.ReadFromJsonAsync<DistilleryResponse>();

            Assert.Multiple(
                () => Assert.Equal(HttpStatusCode.OK, distilleryResponse.StatusCode),
                () => Assert.True(EqualsIgnoringId(DistilleryResponseTestData.Aberfeldy, distillery!)));
        }

        [Fact]
        public async Task When_RequestingDistilleryByIdAndDistilleryDoesNotExist_Expect_NotFoundResponse()
        {
            var endpoint = $"/distilleries/{Guid.NewGuid()}";

            using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
            var response = await httpClient.GetAsync(endpoint);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
}