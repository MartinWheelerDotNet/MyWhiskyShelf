using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.IntegrationTests.Helpers;
using MyWhiskyShelf.IntegrationTests.TestData;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(WorkingFixture))]
public class WebApiWhiskyBottleTests(WorkingFixture fixture)
{
    [Fact]
    public async Task When_AddingWhiskyBottleAndBottleDoesNotExist_Expect_CreatedWithLocationHeaderSet()
    {
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/whisky-bottles",
            WhiskyBottleRequestTestData.GenericCreate);
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var postResponse = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode),
            () => Assert.NotNull(postResponse.Headers.Location));
    }

    [Fact]
    public async Task When_DeleteWhiskyBottleAndWhiskyBottleExists_Expect_NoContent()
    {
        var whiskyBottleResponses = await fixture.SeedWhiskyBottlesAsync([
            WhiskyBottleEntityTestData.Generic("Delete WhiskyBottle")]);
        
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/whisky-bottles/{whiskyBottleResponses.Single().Id}");
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task When_DeleteWhiskyBottleAndWhiskyBottleDoesNotExists_Expect_NotFound()
    {
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/whisky-bottles/{Guid.NewGuid()}");
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task When_UpdateWhiskyBottleAndWhiskyBottleExists_Expect_Ok()
    {
        var whiskyBottleResponses = await fixture.SeedWhiskyBottlesAsync([
            WhiskyBottleEntityTestData.Generic("Update Distillery")
        ]);
        var (name, id) = (whiskyBottleResponses.Single().Name, whiskyBottleResponses.Single().Id);
        
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/whisky-bottles/{id}",
            WhiskyBottleRequestTestData.GenericUpdate with { Name = name, VolumeRemainingCl = 20 });
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

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
        using var httpClient = await fixture.Application.CreateAdminHttpsClientAsync();

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}