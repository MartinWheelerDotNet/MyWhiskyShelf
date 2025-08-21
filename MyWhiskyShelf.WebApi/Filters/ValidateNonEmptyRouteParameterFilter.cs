using MyWhiskyShelf.WebApi.ErrorResults;

namespace MyWhiskyShelf.WebApi.Filters;

public class ValidateNonEmptyRouteParameterFilter(string parameterName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return context.HttpContext.Request.RouteValues.TryGetValue(parameterName, out var value)
               && !string.IsNullOrWhiteSpace(value?.ToString())
            ? await next(context)
            : ValidationProblemResults.MissingOrEmptyRouteParameter(parameterName);
    }
}