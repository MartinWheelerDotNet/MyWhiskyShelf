using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiDistilleriesTests(MyWhiskyShelfFixture fixture)
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_RequestingAllDistilleries_Expect_AllDistilleriesReturned()
    {
        const string endpoint = "/distilleries";
        List<DistilleryResponse> expectedDistilleryResponses =
        [
            DistilleryResponseTestData.Aberargie,
            DistilleryResponseTestData.Aberfeldy,
            DistilleryResponseTestData.Aberlour
        ];
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var addedIds = await DatabaseSeeding.AddDistilleries(
            httpClient,
            DistilleryRequestTestData.Aberargie,
            DistilleryRequestTestData.Aberfeldy,
            DistilleryRequestTestData.Aberlour);

        var response = await httpClient.GetAsync(endpoint);
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();
        await DatabaseSeeding.RemoveDistilleries(httpClient, addedIds);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.All(expectedDistilleryResponses, distillery
                => Assert.Contains(distilleries!, actual => Assertions.EqualsIgnoringId(distillery, actual))),
            () => Assert.All(distilleries!, distillery
                => Assert.NotEqual(Guid.Empty, distillery.Id)));
    }

    [Fact]
    public async Task When_RequestingDistilleryByIdAndDistilleryExists_Expect_CorrectDistilleryReturned()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var addedIds = await DatabaseSeeding.AddDistilleries(httpClient, DistilleryRequestTestData.Aberargie);
        var distilleryEndpoint = $"/distilleries/{addedIds[0]}";

        var distilleryResponse = await httpClient.GetAsync(distilleryEndpoint);
        var distillery = await distilleryResponse.Content.ReadFromJsonAsync<DistilleryResponse>();
        await DatabaseSeeding.RemoveDistilleries(httpClient, addedIds);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, distilleryResponse.StatusCode),
            () => Assert.True(Assertions.EqualsIgnoringId(DistilleryResponseTestData.Aberargie, distillery!)));
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
            DistilleryRequestTestData.Aberargie);

        Assert.Multiple(
            () => Assert.NotNull(response.Headers.Location),
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode));

        await httpClient.DeleteAsync(response.Headers.Location!.OriginalString);
    }

    [Fact]
    public async Task When_RemovingDistilleryAndDistilleryExists_Expect_OkResponse()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var distilleryIds = await DatabaseSeeding.AddDistilleries(httpClient, DistilleryRequestTestData.Aberfeldy);

        var response = await httpClient.DeleteAsync($"/distilleries/{distilleryIds[0]}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_RemovingDistilleryAndDistilleryDoesNotExist_Expect_NotFoundProblemResponse()
    {
        var id = Guid.NewGuid();
        var expectedProblem = new ProblemDetails
        {
            Type = "urn:mywhiskyshelf:errors:distillery-does-not-exist",
            Title = "distillery does not exist.",
            Status = StatusCodes.Status404NotFound,
            Detail = $"Cannot delete distillery '{id}' as it does not exist.",
            Instance = $"/distilleries/{id}"
        };

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.DeleteAsync($"/distilleries/{id}");
        var problemResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.NotFound, response.StatusCode),
            () => Assert.Equivalent(expectedProblem, problemResponse));
    }
}