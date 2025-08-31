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
}