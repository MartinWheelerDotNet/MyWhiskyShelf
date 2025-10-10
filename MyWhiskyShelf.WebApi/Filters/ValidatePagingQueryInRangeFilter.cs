using MyWhiskyShelf.WebApi.ErrorResults;

namespace MyWhiskyShelf.WebApi.Filters;

public class ValidatePagingQueryInRangeFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var hasPage = false;
        var hasAmount = false;
        var page = 0;
        var amount = 0;
        if (context.HttpContext.Request.Query.TryGetValue("page", out var pageString))
            hasPage = int.TryParse(pageString, out page);
         
        if (context.HttpContext.Request.Query.TryGetValue("amount", out var amountString))
            hasAmount = int.TryParse(amountString, out amount);
        
        if (!hasPage && !hasAmount)
            return await next(context);
        
        var errors = ValidatePagingParameters(hasPage, hasAmount, page, amount);
        if (errors.Count > 0) 
            return ValidationProblemResults.PagingParametersAreOutOfRange(errors);
        
        return await next(context);
    }

    private static Dictionary<string, string[]> ValidatePagingParameters(bool hasPage, bool hasAmount, int page, int amount)
    {
        Dictionary<string, string[]> errors = new();
        
        if (hasPage && !hasAmount || !hasPage && hasAmount)
            errors["paging"] = ["Either page and amount should be omitted, or both should be provided"];
        if (hasPage && page < 1) 
            errors["page"] = ["page must be greater than or equal to 1"];
        if (hasAmount && amount is < 1 or > 200) 
            errors["amount"] = ["amount must be between 1 and 200"];
        return errors;
    }
}