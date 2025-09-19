using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.TestData;
using static MyWhiskyShelf.IntegrationTests.Fixtures.WorkingFixture;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiWhiskyBottleTests(WorkingFixture fixture)
{
    private const string WebApiResourceName = "WebApi";

    [Fact]
    public async Task When_AddingWhiskyBottleAndBottleDoesNotExist_Expect_CreatedWithLocationHeaderSet()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/whisky-bottles",
            WhiskyBottleRequestTestData.GenericCreate);

        var postResponse = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode),
            () => Assert.NotNull(postResponse.Headers.Location));
    }

    [Fact]
    public async Task When_DeleteWhiskyBottleAndWhiskyBottleExists_Expect_NoContent()
    {
        var (_, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Post, EntityType.WhiskyBottle);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/whisky-bottles/{id}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task When_DeleteWhiskyBottleAndWhiskyBottleDoesNotExists_Expect_NotFound()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/whisky-bottles/{Guid.NewGuid()}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdateWhiskyBottleAndWhiskyBottleExists_Expect_Ok()
    {
        await fixture.SeedWhiskyBottlesAsync();
        var (name, id) = fixture.GetSeededEntityDetailByTypeAndMethod(HttpMethod.Delete, EntityType.WhiskyBottle);
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/whisky-bottles/{id}",
            WhiskyBottleRequestTestData.GenericUpdate with { Name = name, VolumeRemainingCl = 20 });
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdateWhiskyBottleAndWhiskyBottleDoesNotExist_Expect_NotFoundResponse()
    {
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/whisky-bottles/{Guid.NewGuid()}",
            WhiskyBottleRequestTestData.GenericUpdate);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}