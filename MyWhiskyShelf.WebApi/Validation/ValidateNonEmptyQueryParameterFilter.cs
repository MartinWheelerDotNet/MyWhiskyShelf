using MyWhiskyShelf.WebApi.Endpoints;

namespace MyWhiskyShelf.WebApi.Validation;

public class ValidateNonEmptyQueryParameterFilter(string parameterName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return context.HttpContext.Request.Query.TryGetValue(parameterName, out var value)
               && !string.IsNullOrWhiteSpace(value)
            ? await next(context)
            : ValidationProblemResults.MissingOrEmptyQueryParameter(parameterName);
    }
}