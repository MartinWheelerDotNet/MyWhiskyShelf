using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(BrokenFixture))]
public class InternalServerErrorTests(BrokenFixture fixture)
{
    [Fact]
    public async Task When_CreateNewDistilleryAndErrorOccurs_Expect_Problem()
    {
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Post,
            "/distilleries",
            TestData.DistilleryRequestTestData.GenericCreate with { Name = "Create Error"});

        var response = await httpClient.SendAsync(request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => Assert.Equal("urn:mywhiskyshelf:errors:distillery-create-failed", problemDetails.Type),
            () => Assert.Equal("Failed to create distillery", problemDetails.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status),
            () => Assert.Equal("/distilleries", problemDetails.Instance),
            () => Assert.Contains("An unexpected error occurred", problemDetails.Detail));
    }
    
    [Fact]
    public async Task When_GetDistilleryByIdAndErrorOccurs_Expect_Problem()
    {
        var endpoint = $"/distilleries/{Guid.NewGuid()}";
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        
        var response = await httpClient.GetAsync(endpoint);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => Assert.Equal("urn:mywhiskyshelf:errors:distillery-get-by-id-failed", problemDetails.Type),
            () => Assert.Equal("Failed to get-by-id distillery", problemDetails.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status),
            () => Assert.Equal(endpoint, problemDetails.Instance),
            () => Assert.Contains("An unexpected error occurred", problemDetails.Detail));
    }
    
    [Fact]
    public async Task When_GetAllDistilleriesAndErrorOccurs_Expect_Problem()
    {
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        
        var response = await httpClient.GetAsync("/distilleries");
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => Assert.Equal("urn:mywhiskyshelf:errors:distillery-get-all-failed", problemDetails.Type),
            () => Assert.Equal("Failed to get-all distillery", problemDetails.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status),
            () => Assert.Equal("/distilleries", problemDetails.Instance),
            () => Assert.Contains("An unexpected error occurred", problemDetails.Detail));
    }
    
    [Fact]
    public async Task When_SearchDistilleriesAndErrorOccurs_Expect_Problem()
    {
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        
        var response = await httpClient.GetAsync("/distilleries/search?pattern=anything");
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => Assert.Equal("urn:mywhiskyshelf:errors:distillery-search-failed", problemDetails.Type),
            () => Assert.Equal("Failed to search distillery", problemDetails.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status),
            () => Assert.Equal("/distilleries/search", problemDetails.Instance),
            () => Assert.Contains("An unexpected error occurred", problemDetails.Detail));
    }
    
    [Fact]
    public async Task When_UpdateDistilleryAndErrorOccurs_Expect_Problem()
    {
        var endpoint = $"/distilleries/{Guid.NewGuid()}";
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        var request = IdempotencyHelpers.CreateRequestWithIdempotencyKey(
            HttpMethod.Put,
            endpoint,
            TestData.DistilleryRequestTestData.GenericUpdate with { Name = "Update Distillery"});

        var response = await httpClient.SendAsync(request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => Assert.Equal("urn:mywhiskyshelf:errors:distillery-update-failed", problemDetails.Type),
            () => Assert.Equal("Failed to update distillery", problemDetails.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status),
            () => Assert.Equal(endpoint, problemDetails.Instance),
            () => Assert.Contains("An unexpected error occurred", problemDetails.Detail));
    }
    
    [Fact]
    public async Task When_DeleteDistilleryAndErrorOccurs_Expect_Problem()
    {
        var endpoint = $"/distilleries/{Guid.NewGuid()}";
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        var request = IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(HttpMethod.Delete, endpoint);

        var response = await httpClient.SendAsync(request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => Assert.Equal("urn:mywhiskyshelf:errors:distillery-delete-failed", problemDetails.Type),
            () => Assert.Equal("Failed to delete distillery", problemDetails.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status),
            () => Assert.Equal(endpoint, problemDetails.Instance),
            () => Assert.Contains("An unexpected error occurred", problemDetails.Detail));
    }
}