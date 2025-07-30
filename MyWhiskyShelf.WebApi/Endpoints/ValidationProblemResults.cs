using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.WebApi.Endpoints;

public static class ValidationProblemResults
{
    #region WhiskyBottle Validation Problem Results

    public static IResult WhiskyBottleValidationProblemResults()
    {
        return Results.ValidationProblem(
            title: "A validation error occurred when trying to add a whisky bottle.",
            type: "urn:mywhiskyshelf:validation-errors:whisky-bottle",
            errors: new Dictionary<string, string[]>
            {
                [nameof(WhiskyBottle)] =
                [
                    "An error occurred trying to add the whisky bottle to the database. "
                    + "Ensure all required fields have been set."
                ]
            });
    }

    #endregion

    #region Endpoint Validation Problem Results

    public static IResult MissingOrEmptyQueryParameter(string parameterName)
    {
        return Results.ValidationProblem(
            title: "Missing or empty query parameters",
            type: "urn:mywhiskyshelf:validation-errors:query-parameter",
            errors: new Dictionary<string, string[]>
            {
                { parameterName, [$"Query parameter '{parameterName}' is required and cannot be empty."] }
            });
    }

    public static IResult MissingOrEmptyRouteParameter(string parameterName)
    {
        return Results.ValidationProblem(
            title: "Missing or empty route parameters",
            type: "urn:mywhiskyshelf:validation-errors:route-parameter",
            errors: new Dictionary<string, string[]>
            {
                { parameterName, [$"Route parameter '{parameterName}' is required and cannot be empty."] }
            });
    }

    #endregion
}