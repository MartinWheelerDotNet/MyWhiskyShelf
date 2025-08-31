using System.Net.Http.Json;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.TestData;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;
using static MyWhiskyShelf.IntegrationTests.Fixtures.MyWhiskyShelfFixture;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiDistilleriesTests(MyWhiskyShelfFixture fixture) : IClassFixture<MyWhiskyShelfFixture>
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_GettingAllDistilleries_Expect_AllDistilleriesReturned()
    {
        await fixture.SeedDistilleriesAsync();
        const string endpoint = "/distilleries";
        var expectedDistilleryResponses = fixture
            .GetSeededEntityDetailsByType(EntityType.Distillery)
            .Select((kvp, _) => DistilleryResponseTestData.GenericResponse(kvp.Id) with { Name = kvp.Name });

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.GetAsync(endpoint);
        var distilleries = await response.Content.ReadFromJsonAsync<List<DistilleryResponse>>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.All(expectedDistilleryResponses, distillery
                => Assert.Contains(distilleries!, actual => distillery == actual)),
            () => Assert.All(distilleries!, distillery
                => Assert.NotEqual(Guid.Empty, distillery.Id)));
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryExists_Expect_CorrectDistilleryReturned()
    {
        await fixture.SeedDistilleriesAsync();
        var (name, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Post, EntityType.Distillery);

        var expectedResponse = DistilleryResponseTestData.GenericResponse(id) with { Name = name };

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.GetAsync($"/distilleries/{id}");
        var distilleryResponse = await response.Content.ReadFromJsonAsync<DistilleryResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, response.StatusCode),
            () => Assert.Equal(expectedResponse, distilleryResponse!));
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryDoesNotExist_Expect_NotFoundResponse()
    {
        var endpoint = $"/distilleries/{Guid.NewGuid()}";
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task When_AddingDistilleryAndDistilleryDoesNotExist_Expect_CreatedWithLocationHeaderSet()
    {
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.GenericCreate with { Name = "Distillery D" });
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.NotNull(response.Headers.Location),
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode));
    }

    [Fact]
    public async Task When_AddingDistilleryAndDistilleryAlreadyExists_Expect_Conflict()
    {
        var (name, _) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Get, EntityType.Distillery);

        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.GenericCreate with { Name = name });
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryExists_Expect_NoContent()
    {
        await fixture.SeedDistilleriesAsync();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var (_, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Delete, EntityType.Distillery);
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{id}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryDoesNotExist_Expect_NotFound()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{Guid.NewGuid()}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryExists_Expect_OkResponse()
    {
        await fixture.SeedDistilleriesAsync();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var (_, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Put, EntityType.Distillery);
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{id}",
            DistilleryRequestTestData.GenericUpdate);

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryDoesNotExist_Expect_NotFoundResponse()
    {
        var distilleryId = Guid.NewGuid();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{distilleryId}",
            DistilleryRequestTestData.GenericUpdate with { Name = "Update Distillery" });

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}