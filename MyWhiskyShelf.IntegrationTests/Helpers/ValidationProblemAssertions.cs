using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.IntegrationTests.Helpers;

public static class ValidationProblemAssertions
{
    public static async Task AssertValidationProblemAsync(
        HttpResponseMessage response,
        int expectedStatus,
        string expectedTitle,
        string expectedType,
        IDictionary<string, string[]> expectedErrors)
    {
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(expectedStatus, problem.Status);
        Assert.Equal(expectedTitle, problem.Title);
        Assert.Equal(expectedType, problem.Type);

        Assert.Equal(expectedErrors.Count, problem.Errors.Count);
        foreach (var kv in expectedErrors)
        {
            Assert.True(problem.Errors.ContainsKey(kv.Key), $"Missing validation key '{kv.Key}'.");
            Assert.Equal(kv.Value, problem.Errors[kv.Key]);
        }
    }
}