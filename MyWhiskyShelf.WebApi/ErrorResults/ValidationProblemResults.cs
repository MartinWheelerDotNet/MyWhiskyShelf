namespace MyWhiskyShelf.WebApi.ErrorResults;

public static class ValidationProblemResults
{
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

    public static IResult MissingOrInvalidIdempotencyKey()
    {
        return Results.ValidationProblem(
            title: "Missing or empty idempotency key",
            type: "urn:mywhiskyshelf:validation-errors:idempotency-key",
            errors: new Dictionary<string, string[]>
            {
                { "idempotencyKey", ["Header value 'idempotency-key' is required and must be an non-empty UUID"] }
            });
    }

    public static IResult InvalidPagingParameters(Dictionary<string, string[]> errors)
    {
        return Results.ValidationProblem(
            title: "Paging parameters are out of range",
            type: "urn:mywhiskyshelf:validation-errors:paging",
            errors: errors);
    }

    public static IResult CountryNotFound(Guid countryId)
    {
        return Results.ValidationProblem(
            title: "Provided country not found by Id",
            type: "urn:mywhiskyshelf:validation-errors:country-not-found",
            errors: new Dictionary<string, string[]>
            {
                { "countryId", [$"The provided countryId {countryId} does not exist in the database."] }
            });
    }
}