using Microsoft.AspNetCore.Mvc;

namespace MyWhiskyShelf.WebApi.Filters;

public class ValidateNonEmptyRouteParameterFilter(string parameterName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var routeValue = httpContext.Request.RouteValues[parameterName]?.ToString();

        if (string.IsNullOrWhiteSpace(routeValue))
            return Results.Problem(new ProblemDetails
            {
                Type = "urn:mywhiskyshelf:errors:missing-or-invalid-query-pattern",
                Title = "Missing or invalid query parameter",
                Status = StatusCodes.Status400BadRequest,
                Detail = $"Query parameter '{parameterName}' is required and cannot be empty.",
                Instance = httpContext.Request.Path
            });

        return await next(context);
    }
}