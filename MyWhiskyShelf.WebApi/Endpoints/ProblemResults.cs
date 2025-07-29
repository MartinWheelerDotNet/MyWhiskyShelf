using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static class ProblemResults
{
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


    public static IResult DistilleryNotFound(string distilleryName, HttpContext httpContext)
    {
        return Results.Problem(
            new ProblemDetails
            {
                Type = "urn:mywhiskyshelf:errors:distillery-does-not-exist",
                Title = "Distillery does not exist.",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Cannot remove distillery '{distilleryName}' as it does not exist.",
                Instance = httpContext.Request.Path
            });
    }


    public static IResult MissingOrEmptyQueryParameter(string parameterName)
    {
        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                { parameterName, [$"Query parameter '{parameterName}' is required and cannot be empty."] }
            },
            type: "urn:mywhiskyshelf:errors:request-validation-error");
    }
}