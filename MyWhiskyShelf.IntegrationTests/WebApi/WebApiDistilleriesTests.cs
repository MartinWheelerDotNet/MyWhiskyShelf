using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers;
using MyWhiskyShelf.TestHelpers.Data;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiDistilleriesTests(MyWhiskyShelfFixture fixture) : IAsyncLifetime
{
    private const string WebApiResourceName = "WebApi";

    public async Task InitializeAsync()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        await DatabaseSeeding.SeedDatabase(httpClient);
    }
    
    #region GET - Create Distillery Tests

    [Fact]
    public async Task When_GettingAllDistilleries_Expect_AllDistilleriesReturned()
    {
        const string endpoint = "/distilleries";
        List<DistilleryResponse> expectedDistilleryResponses =
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
            () => Assert.All(expectedDistilleryResponses, distillery
                => Assert.Contains(distilleries!, actual => Assertions.EqualsIgnoringId(distillery, actual))),
            () => Assert.All(distilleries!, distillery
                => Assert.NotEqual(Guid.Empty, distillery.Id)));
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryExists_Expect_CorrectDistilleryReturned()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var distilleryDetails = await httpClient
            .GetFromJsonAsync<List<DistilleryNameDetails>>("/distilleries/name/search?pattern=aberargie");

        var distilleryResponse = await httpClient.GetAsync($"/distilleries/{distilleryDetails![0].Id}");
        var distillery = await distilleryResponse.Content.ReadFromJsonAsync<DistilleryResponse>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.OK, distilleryResponse.StatusCode),
            () => Assert.True(Assertions.EqualsIgnoringId(DistilleryResponseTestData.Aberargie, distillery!)));
    }

    [Fact]
    public async Task When_GettingDistilleryByIdAndDistilleryDoesNotExist_Expect_NotFoundResponse()
    {
        var endpoint = $"/distilleries/{Guid.NewGuid()}";
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.GetAsync(endpoint);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region POST Request Tests

    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \t")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task When_PostingDistilleryWithInvalidIdempotencyKey_Expect_ValidationProblem(string? idempotencyKey)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = CreatePostRequestWithIdempotencyKey(idempotencyKey);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_PostingDistilleryWithMissingIdempotencyKey_Expect_ValidationProblem()
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.PostAsJsonAsync("/distilleries", DistilleryRequestTestData.Aberargie);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_PostingDistilleryTwiceWithSameIdempotencyKey_Expect_TheSameResultReturned()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        var initialRequest = CreatePostRequestWithIdempotencyKey(idempotencyKey);
        var resendRequest = CreatePostRequestWithIdempotencyKey(idempotencyKey);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var initialResponse = await httpClient.SendAsync(initialRequest);
        var resendResponse = await httpClient.SendAsync(resendRequest);

        Assert.Equivalent(initialResponse, resendResponse);
    }

    [Fact]
    public async Task When_PostingDistilleryAndDistilleryDoesNotExist_Expect_CreatedWithLocationHeaderSet()
    {
        var request = CreatePostRequestWithIdempotencyKey();

        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var response = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.NotNull(response.Headers.Location),
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode));
    }
    
    private static HttpRequestMessage CreatePostRequestWithIdempotencyKey(string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/distilleries")
        {
            Content = JsonContent.Create(DistilleryRequestTestData.NewDistillery)
        };
        request.Headers.Add("Idempotency-Key", idempotencyKey ?? Guid.NewGuid().ToString());
        return request;
    }

    #endregion

    [Fact]
    public async Task When_RemovingDistilleryAndDistilleryExists_Expect_NoContentResponse()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var distilleryDetails = await httpClient
            .GetFromJsonAsync<List<DistilleryNameDetails>>("/distilleries/name/search?pattern=aberargie");
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/distilleries/{distilleryDetails![0].Id}");
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task When_RemovingDistilleryAndDistilleryDoesNotExist_Expect_NoContent()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/distilleries/{Guid.NewGuid()}");
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private static ValidationProblemDetails CreateIdempotencyKeyValidationProblem() =>
        new()
        {
            Type = "urn:mywhiskyshelf:validation-errors:idempotency-key",
            Title = "Missing or empty idempotency key",
            Status = StatusCodes.Status400BadRequest,
            Errors = new Dictionary<string, string[]>
            {
                { "idempotencyKey", ["Header value 'idempotency-key' is required and must be an non-empty UUID"] }
            }
        };
    
    public async Task DisposeAsync()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        await DatabaseSeeding.ClearDatabase(httpClient);
    }
}