using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.IntegrationTests.Fixtures;
using MyWhiskyShelf.TestHelpers.Data;
using static MyWhiskyShelf.TestHelpers.Assertions;

namespace MyWhiskyShelf.IntegrationTests.WebApi;

[Collection("AspireTests")]
public class WebApiWhiskyBottleTests(MyWhiskyShelfFixture fixture)
{
    
    private const string WebApiResourceName = "WebApi";
    private const string Endpoint = "/whisky-bottle";
    private HttpClient CreateClient() => fixture.Application.CreateHttpClient(WebApiResourceName);
    
    [Fact]
    public async Task When_AddWhiskyBottle_Expect_WhiskyBottleIsCreatedWithLocationHeaderSet()
    {
        using var httpClient = CreateClient();
        var postResponse = await httpClient.PostAsJsonAsync(Endpoint, WhiskyBottleRequestTestData.AllValuesPopulated);
        
        await httpClient.DeleteAsync(postResponse.Headers.Location);
        
        var parts = postResponse.Headers.Location!.OriginalString.Trim('/').Split("/");
        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode),
            () => Assert.Equal("whisky-bottle", parts[0]),
            () => AssertIsGuidAndNotEmpty(parts[1]));
    }

    [Fact]
    public async Task When_AddWhiskyBottleWithInvalidData_Expect_ValidationProblemDetails()
    {
        var expectedProblem = new ValidationProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Type = "urn:mywhiskyshelf:validation-errors:whisky-bottle",
            Status = 400,
            Errors = new Dictionary<string, string[]>
            {
                ["WhiskyBottleRequest"] = ["An error occurred trying to add the whisky bottle to the database."]
            }
        };
        var invalidBottle = WhiskyBottleRequestTestData.AllValuesPopulated with { Name = null! };

        using var httpClient = CreateClient();
        var addResponse = await httpClient.PostAsJsonAsync(Endpoint, invalidBottle);
        var problemDetails = await addResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.BadRequest, addResponse.StatusCode),
            () => Assert.Equivalent(expectedProblem, problemDetails));
    }

    [Fact]
    public async Task When_DeleteWhiskyBottleAndBottleDoesNotExist_Expect_NotFoundProblemDetails()
    {
        var id = Guid.NewGuid();
        var url = $"/whisky-bottle/{id}";
        var expectedProblem = CreateExpectedResourceNotFound("whisky-bottle", "delete", id, $"/whisky-bottle/{id}");
         
        using var httpClient = CreateClient();
        var deleteResponse = await httpClient.DeleteAsync(url);
        var problemResponse = await deleteResponse.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode),
            () => Assert.Equivalent(expectedProblem, problemResponse));
    }

    [Fact]
    public async Task When_DeleteWhiskyBottleAndBottleExists_Expect_Ok()
    {
        using var httpClient = CreateClient();
        var createResponse = await httpClient.PostAsJsonAsync(Endpoint, WhiskyBottleRequestTestData.AllValuesPopulated);

        var deleteResponse = await httpClient.DeleteAsync(createResponse.Headers.Location);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task When_UpdateWhiskyBottleAndWhiskyBottleExists_Expect_Ok()
    {
        using var httpClient = CreateClient();
        var createResponse = await httpClient.PostAsJsonAsync(Endpoint, WhiskyBottleRequestTestData.AllValuesPopulated);
        
        var updatedBottle = WhiskyBottleRequestTestData.AllValuesPopulated with { VolumeRemainingCl = 20 };
        var updateResponse = await httpClient.PutAsJsonAsync(createResponse.Headers.Location, updatedBottle);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }
    
    [Fact]
    public async Task When_UpdateWhiskyBottleAndWhiskyBottleDoesNotExist_Expect_NotFoundProblemDetails()
    {
        var id = Guid.NewGuid();
        var url = $"/whisky-bottle/{id}";
        var expectedProblem = CreateExpectedResourceNotFound("whisky-bottle", "update", id, $"/whisky-bottle/{id}"); 
        
        using var httpClient = CreateClient();
        var updateResponse = await httpClient.PutAsJsonAsync(url, WhiskyBottleRequestTestData.AllValuesPopulated);
        var problemDetails = await updateResponse.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Multiple(
            () => Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode),
            () => Assert.Equivalent(expectedProblem, problemDetails));
    }
    
    #region Helpers
    
    private static ProblemDetails CreateExpectedResourceNotFound(
        string resourceName,
        string action,
        Guid resourceId,
        string instance)
    {
        return new ProblemDetails
        {
            Type = $"urn:mywhiskyshelf:errors:{resourceName}-does-not-exist",
            Title = $"{resourceName} does not exist.",
            Status = StatusCodes.Status404NotFound,
            Detail = $"Cannot {action} {resourceName} '{resourceId}' as it does not exist.",
            Instance = instance
        };
    }
    
    #endregion
}