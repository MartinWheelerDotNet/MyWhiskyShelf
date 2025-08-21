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

    #region Idempotency Tests
    
    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \t")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task When_AddingDistilleryWithInvalidIdempotencyKey_Expect_ValidationProblem(string? idempotencyKey)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = CreateRequestWithIdempotencyKey(
            HttpMethod.Post, 
            "/distilleries",
            DistilleryRequestTestData.NewDistillery,
            idempotencyKey);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_AddingDistilleryWithMissingIdempotencyKey_Expect_ValidationProblem()
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.PostAsJsonAsync("/distilleries", DistilleryRequestTestData.Aberargie);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_AddingDistilleryTwiceWithSameIdempotencyKey_Expect_TheSameResultReturned()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        var initialRequest = CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.NewDistillery,
            idempotencyKey);
        var resendRequest = CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.NewDistillery,
            idempotencyKey);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var initialResponse = await httpClient.SendAsync(initialRequest);
        var resendResponse = await httpClient.SendAsync(resendRequest);

        Assert.Equivalent(initialResponse, resendResponse);
    }
    
    #endregion

    [Fact]
    public async Task When_AddingDistilleryAndDistilleryDoesNotExist_Expect_CreatedWithLocationHeaderSet()
    {
        var request = CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.NewDistillery);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);

        Assert.Multiple(
            () => Assert.NotNull(response.Headers.Location),
            () => Assert.Equal(HttpStatusCode.Created, response.StatusCode));
    }
    
    [Fact]
    public async Task When_AddingDistilleryAndDistilleryAlreadyExists_Expect_ConflictProblemDetails()
    {
        var expectedProblemDetails = new ProblemDetails
        {
            Type = "urn:mywhiskyshelf:errors:distillery-already-exists",
            Title = "distillery already exists.",
            Status = StatusCodes.Status409Conflict,
            Detail = "Cannot add distillery 'Aberargie' as it already exists.",
            Instance = "/distilleries"
        };
        var request = CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            DistilleryRequestTestData.Aberargie);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equivalent(expectedProblemDetails, problemDetails);
    }

    #endregion

    #region DELETE - Remove Distillery Tests
    
    #region Idempotency Tests
    
    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \t")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task When_DeletingDistilleryWithInvalidIdempotencyKey_Expect_ValidationProblem(string? idempotencyKey)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = CreateRequestWithIdempotencyKey(
            HttpMethod.Delete,
            $"/distilleries/{Guid.NewGuid()}",
            idempotencyKey: idempotencyKey);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_DeletingDistilleryWithMissingIdempotencyKey_Expect_ValidationProblem()
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.DeleteAsync($"/distilleries/{Guid.NewGuid()}");
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_DeletingDistilleryTwiceWithSameIdempotencyKey_Expect_TheSameResultReturned()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var distilleryDetails = await httpClient
            .GetFromJsonAsync<List<DistilleryNameDetails>>("/distilleries/names?pattern=aberfeldy");
        var initialRequest = CreateRequestWithIdempotencyKey(
            HttpMethod.Delete, 
            $"/distilleries/{distilleryDetails![0].Id}", 
            idempotencyKey: idempotencyKey);
        var resendRequest = CreateRequestWithIdempotencyKey(
            HttpMethod.Delete, 
            $"/distilleries/{distilleryDetails[0].Id}", 
            idempotencyKey: idempotencyKey);
        

        var initialResponse = await httpClient.SendAsync(initialRequest);
        var resendResponse = await httpClient.SendAsync(resendRequest);

        Assert.Equivalent(initialResponse, resendResponse);
    }
    
    #endregion
    
    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryExists_Expect_NoContentResponse()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var distilleryDetails = await httpClient
            .GetFromJsonAsync<List<DistilleryNameDetails>>("/distilleries/name/search?pattern=aberargie");
        var request = CreateRequestWithIdempotencyKey(HttpMethod.Delete, $"/distilleries/{distilleryDetails![0].Id}");
        
        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task When_DeletingDistilleryAndDistilleryDoesNotExist_Expect_NoContent()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = CreateRequestWithIdempotencyKey(HttpMethod.Delete, $"/distilleries/{Guid.NewGuid()}");

        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    #endregion

    #region PUT - Update Distillery Tests

    #region Idempotency Tests
    
    [Theory]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("    ")]
    [InlineData("\t \t")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task When_UpdatingDistilleryWithInvalidIdempotencyKey_Expect_ValidationProblem(string? idempotencyKey)
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        var request = CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{Guid.NewGuid()}",
            DistilleryRequestTestData.NewDistillery,
            idempotencyKey);
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient.SendAsync(request);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_UpdatingDistilleryWithMissingIdempotencyKey_Expect_ValidationProblem()
    {
        var expectedValidationProblem = CreateIdempotencyKeyValidationProblem();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);

        var response = await httpClient
            .PutAsJsonAsync($"/distilleries/{Guid.NewGuid()}", DistilleryRequestTestData.NewDistillery);
        var validationProblem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equivalent(expectedValidationProblem, validationProblem);
    }

    [Fact]
    public async Task When_UpdatingDistilleryTwiceWithSameIdempotencyKey_Expect_TheSameResultReturned()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var distilleryDetails = await httpClient
            .GetFromJsonAsync<List<DistilleryNameDetails>>("/distilleries/names?pattern=aberfeldy"); 
        var initialRequest = CreateRequestWithIdempotencyKey(
            HttpMethod.Put, 
            $"/distilleries/{distilleryDetails![0].Id}",
            DistilleryRequestTestData.NewDistillery,
            idempotencyKey: idempotencyKey);
        var resendRequest = CreateRequestWithIdempotencyKey(
            HttpMethod.Put, 
            $"/distilleries/{distilleryDetails[0].Id}",
            DistilleryRequestTestData.NewDistillery,
            idempotencyKey: idempotencyKey);
        

        var initialResponse = await httpClient.SendAsync(initialRequest);
        var resendResponse = await httpClient.SendAsync(resendRequest);

        Assert.Equivalent(initialResponse, resendResponse);
    }
    
    #endregion
    
    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryExists_Expect_NoContentResponse()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var distilleryDetails = await httpClient
            .GetFromJsonAsync<List<DistilleryNameDetails>>("/distilleries/names/?pattern=aberfeldy");
        var request = CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{distilleryDetails![0].Id}",
            DistilleryRequestTestData.NewDistillery);
       
        var response = await httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task When_UpdatingDistilleryAndDistilleryDoesNotExist_Expect_NotFoundProblemDetails()
    {
        var distilleryId = Guid.NewGuid();
        var expectedProblemDetails = new ProblemDetails
        {
            Type = "urn:mywhiskyshelf:errors:distillery-does-not-exist",
            Title = "distillery does not exist.",
            Status = StatusCodes.Status404NotFound,
            Detail = $"Cannot update distillery '{distilleryId}' as it does not exist.",
            Instance = $"/distilleries/{distilleryId}"
        };
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        var request = CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            $"/distilleries/{distilleryId}",
            DistilleryRequestTestData.NewDistillery);
       
        var response = await httpClient.SendAsync(request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equivalent(expectedProblemDetails, problemDetails);
    }

    #endregion

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
    
    private static HttpRequestMessage CreateRequestWithIdempotencyKey(
        HttpMethod method, 
        string endpoint,
        DistilleryRequest? distilleryRequest = null,
        string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, endpoint);
        if (distilleryRequest is not null)
            request.Content = JsonContent.Create(distilleryRequest);
        
        request.Headers.Add("Idempotency-Key", idempotencyKey ?? Guid.NewGuid().ToString());
        return request;
    }
    
    public async Task DisposeAsync()
    {
        using var httpClient = fixture.Application.CreateHttpClient(WebApiResourceName);
        await DatabaseSeeding.ClearDatabase(httpClient);
    }
}