using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.WebApi.ErrorResults;

internal static class ProblemResults
{
    #region CreateDistilleryRequest Problem Results

    public static IResult ResourceAlreadyExists(string resourceName, string resourceIdentifier, HttpContext httpContext)
    {
        return Results.Problem(
            new ProblemDetails
            {
                Type = $"urn:mywhiskyshelf:errors:{resourceName}-already-exists",
                Title = $"{resourceName} already exists.",
                Status = StatusCodes.Status409Conflict,
                Detail = $"Cannot add {resourceName} '{resourceIdentifier} as it already exists.",
                Instance = httpContext.Request.Path
            });
    }


    public static IResult ResourceNotFound(string resourceName, Guid resourceIdentifier, HttpContext httpContext)
    {
        return Results.Problem(
            new ProblemDetails
            {
                Type = $"urn:mywhiskyshelf:errors:{resourceName}-does-not-exist",
                Title = $"{resourceName} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Cannot remove {resourceName} '{resourceIdentifier}' as it does not exist.",
                Instance = httpContext.Request.Path
            });
    }

    #endregion
}