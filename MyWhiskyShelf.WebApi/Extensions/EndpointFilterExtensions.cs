using System.Diagnostics.CodeAnalysis;
using MyWhiskyShelf.WebApi.Filters;
using MyWhiskyShelf.WebApi.Interfaces;

// ReSharper disable UnusedMethodReturnValue.Global
namespace MyWhiskyShelf.WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder RequiresNonEmptyRouteParameter(
        this RouteHandlerBuilder routeHandlerBuilder,
        string parameterName)
    {
        return routeHandlerBuilder.AddEndpointFilter(new ValidateNonEmptyRouteParameterFilter(parameterName));
    }

    public static RouteHandlerBuilder RequiresIdempotencyKey(this RouteHandlerBuilder routeHandlerBuilder)
    {
        return routeHandlerBuilder.AddEndpointFilter(async (context, next) =>
        {
            var idempotencyService = context.HttpContext.RequestServices
                .GetRequiredService<IIdempotencyService>();

            var filter = new IdempotencyKeyFilter(idempotencyService);
            return await filter.InvokeAsync(context, next);
        });
    }

    public static RouteHandlerBuilder UsesCursorPagingResponse(this RouteHandlerBuilder routeHandlerBuilder)
    {
        return routeHandlerBuilder.AddEndpointFilter(async (context, next) =>
        {
            var filter = new ValidateCursorQueryInRangeFilter();
            return await filter.InvokeAsync(context, next);
        });
    }
}