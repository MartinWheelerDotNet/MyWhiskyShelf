using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.WebApi.ProblemResults;

internal static class ProblemResults
{
    #region DistilleryRequest Problem Results

    public static IResult DistilleryAlreadyExists(string distilleryName, HttpContext httpContext)
    {
        return Results.Problem(
            new ProblemDetails
            {
                Type = "urn:mywhiskyshelf:errors:distillery-already-exists",
                Title = "Distillery already exists.",
                Status = StatusCodes.Status409Conflict,
                Detail = $"Cannot add distillery '{distilleryName} as it already exists.",
                Instance = httpContext.Request.Path
            });
    }


    public static IResult DistilleryNotFound(Guid distilleryId, HttpContext httpContext)
    {
        return Results.Problem(
            new ProblemDetails
            {
                Type = "urn:mywhiskyshelf:errors:distillery-does-not-exist",
                Title = "Distillery does not exist.",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Cannot remove distillery '{distilleryId}' as it does not exist.",
                Instance = httpContext.Request.Path
            });
    }

    #endregion
}