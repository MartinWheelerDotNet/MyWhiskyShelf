using MyWhiskyShelf.WebApi.ErrorResults;

namespace MyWhiskyShelf.WebApi.Validation;

public class ValidateNonEmptyRouteParameterFilter(string parameterName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var routeValue = httpContext.Request.RouteValues[parameterName]?.ToString();

        if (string.IsNullOrWhiteSpace(routeValue))
            return ValidationProblemResults.MissingOrEmptyRouteParameter(parameterName);

        return await next(context);
    }
}