using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.WebApi.ErrorResults;

internal static class ProblemResults
{
    public static IResult InternalServerError(string name, string action, string traceId, string path)
    {
        return Results.Problem(new ProblemDetails
        {
            Type = $"urn:mywhiskyshelf:errors:{name}-{action}-failed",
            Title = $"Failed to {action} {name}",
            Status = StatusCodes.Status500InternalServerError,
            Detail = $"An unexpected error occurred. (TraceId: {traceId})",
            Instance = path
        });
    }
}