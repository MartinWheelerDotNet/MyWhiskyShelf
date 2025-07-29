using MyWhiskyShelf.WebApi.Endpoints;

namespace MyWhiskyShelf.WebApi.Filters;

public class ValidateNonEmptyQueryParameterFilter(string parameterName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return context.HttpContext.Request.Query.TryGetValue(parameterName, out var value)
               && !string.IsNullOrWhiteSpace(value)
            ? await next(context)
            : ProblemResults.MissingOrEmptyQueryParameter(parameterName);
    }
}