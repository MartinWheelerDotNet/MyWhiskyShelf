using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.WebApi.ErrorResults;

internal static class ProblemResults
{
    #region DistilleryRequest Problem Results

    public static IResult ResourceAlreadyExists(string resource, string name, HttpContext httpContext)
    {
        return Results.Problem(
            new ProblemDetails
            {
                Type = $"urn:mywhiskyshelf:errors:{resource}-already-exists",
                Title = $"{resource} already exists.",
                Status = StatusCodes.Status409Conflict,
                Detail = $"Cannot add {resource} '{name}' as it already exists.",
                Instance = httpContext.Request.Path
            });
    }


    public static IResult ResourceNotFound(
        string name,
        string action,
        Guid resourceId,
        HttpContext httpContext)
    {
        return Results.Problem(
            new ProblemDetails
            {
                Type = $"urn:mywhiskyshelf:errors:{name}-does-not-exist",
                Title = $"{name} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Cannot {action} {name} '{resourceId}' as it does not exist.",
                Instance = httpContext.Request.Path
            });
    }

    #endregion
}