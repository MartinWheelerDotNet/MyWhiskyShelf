using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.WebApi.ErrorResults;

public static class ValidationProblemResults
{
    #region WhiskyBottleRequest Validation Problem Results

    public static IResult WhiskyBottleValidationProblemResults()
    {
        return Results.ValidationProblem(
            title: "One or more validation errors occurred.",
            type: "urn:mywhiskyshelf:validation-errors:whisky-bottle",
            errors: new Dictionary<string, string[]>
            {
                [nameof(WhiskyBottleRequest)] = ["An error occurred trying to add the whisky bottle to the database."]
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

    public static IResult MissingIdempotencyKey()
    {
        return Results.ValidationProblem(
            title: "Missing or empty idempotency key",
            type: "urn:mywhiskyshelf:validation-errors:route-parameter",
            errors: new Dictionary<string, string[]>
            {
                { "idempotencyKey", ["Header value 'idempotency-key' is required and cannot be an empty UUID"] }
            }); 
    }

    #endregion
}