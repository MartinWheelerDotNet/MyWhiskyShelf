using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection(nameof(BrokenFixture))]
public class InternalServerErrorTests(BrokenFixture fixture)
{
    public static TheoryData<string, string, string, string, RequestBodyWrapper?> InvalidRequestData()
    {
        var data = new TheoryData<string, string, string, string, RequestBodyWrapper?>
        {
            {
                "distillery", "get-by-id", $"/distilleries/{Guid.NewGuid()}", HttpMethod.Get.Method, null
            },
            {
                "distillery", "get-all", "/distilleries", HttpMethod.Get.Method, null
            },
            {
                "distillery", "search", "/distilleries/search?pattern=anything", HttpMethod.Get.Method, null
            },
            {
                "distillery", "delete", $"/distilleries/{Guid.NewGuid()}", HttpMethod.Delete.Method, null
            },
            {
                "distillery", "create", "/distilleries", HttpMethod.Post.Method,
                new RequestBodyWrapper(TestData.DistilleryRequestTestData.GenericCreate with { Name = "Create Error"}) 
            },
            {
                "distillery", "update", $"/distilleries/{Guid.NewGuid()}", HttpMethod.Put.Method,
                new RequestBodyWrapper(TestData.DistilleryRequestTestData.GenericUpdate with { Name = "Update Error"}) 
            },
            {
                "whisky-bottle", "delete", $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Delete.Method, null
            },
            {
                "whisky-bottle", "create", "/whisky-bottles", HttpMethod.Post.Method,
                new RequestBodyWrapper(TestData.WhiskyBottleRequestTestData.GenericCreate with { Name = "Create Error"}) 
            },
            {
                "whisky-bottle", "update", $"/whisky-bottles/{Guid.NewGuid()}", HttpMethod.Put.Method,
                new RequestBodyWrapper(TestData.WhiskyBottleRequestTestData.GenericUpdate with { Name = "Update Error"}) 
            }
        };

        return data;
    }
    
    [Theory]
    [MemberData(nameof(InvalidRequestData), DisableDiscoveryEnumeration = true)]
    public async Task When_InternalServerErrorOccurs_Expect_Problem(
        string name,
        string action,
        string instance,
        string httpMethod,
        RequestBodyWrapper? requestBody)
    {
        
        using var httpClient = fixture.Application.CreateHttpClient("WebApi");
        
        var request = requestBody is null 
            ? IdempotencyHelpers.CreateNoBodyRequestWithIdempotencyKey(HttpMethod.Parse(httpMethod), instance)
            : IdempotencyHelpers.CreateRequestWithIdempotencyKey(
                HttpMethod.Parse(httpMethod),
                instance,
                requestBody.Value);

        var response = await httpClient.SendAsync(request);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        Assert.NotNull(problemDetails);
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode),
            () => AssertProblem(name, action, instance.Split('?')[0], problemDetails));
    }
    
    private static void AssertProblem(string name, string action, string instance, ProblemDetails actual)
    {
        Assert.NotNull(actual);
        Assert.Multiple(
            () => Assert.Equal($"urn:mywhiskyshelf:errors:{name}-{action}-failed", actual.Type),
            () => Assert.Equal($"Failed to {action} {name}", actual.Title),
            () => Assert.Equal(StatusCodes.Status500InternalServerError, actual.Status),
            () => Assert.Equal(instance, actual.Instance),
            () => Assert.Contains("An unexpected error occurred", actual.Detail));
    }
}