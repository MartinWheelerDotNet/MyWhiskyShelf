using MyWhiskyShelf.WebApi.ErrorResults;

namespace MyWhiskyShelf.WebApi.Filters;

public class ValidateCursorQueryInRangeFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var query = context.HttpContext.Request.Query;

        if (!query.TryGetValue("amount", out var amountValues) || !int.TryParse(amountValues, out var amount))
            return await next(context);

        if (amount is < 1 or > 200)
            return ValidationProblemResults.InvalidPagingParameters(new Dictionary<string, string[]>
            {
                ["amount"] = ["amount must be between 1 and 200"]
            });

        return await next(context);
    }
}