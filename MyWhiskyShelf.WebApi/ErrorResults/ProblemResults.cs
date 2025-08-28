using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.WebApi.ErrorResults;

internal static class ProblemResults
{
    public static ProblemDetails InternalServerError(string name, string action, string traceId, string path)
    {
        return new ProblemDetails
        {
            Type = $"urn:mywhiskyshelf:errors:{name}-{action}-failed",
            Title = $"Failed to created {name}",
            Status = StatusCodes.Status500InternalServerError,
            Detail = $"An unexpected error occurred. (TraceId: {traceId})",
            Instance = path
        };
    }
}