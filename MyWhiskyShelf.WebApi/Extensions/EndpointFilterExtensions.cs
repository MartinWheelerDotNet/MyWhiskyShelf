using System.Diagnostics.CodeAnalysis;
using MyWhiskyShelf.WebApi.Filters;
using MyWhiskyShelf.WebApi.Interfaces;

namespace MyWhiskyShelf.WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class EndpointFilterExtensions
{
    extension(RouteHandlerBuilder routeHandlerBuilder)
    {
        public RouteHandlerBuilder RequiresNonEmptyRouteParameter(string parameterName)
        {
            return routeHandlerBuilder.AddEndpointFilter(new ValidateNonEmptyRouteParameterFilter(parameterName));
        }

        public RouteHandlerBuilder RequiresIdempotencyKey()
        {
            return routeHandlerBuilder.AddEndpointFilter(async (context, next) =>
            {
                var idempotencyService = context.HttpContext.RequestServices
                    .GetRequiredService<IIdempotencyService>();

                var filter = new IdempotencyKeyFilter(idempotencyService);
                return await filter.InvokeAsync(context, next);
            });
        }
    }

    extension(RouteHandlerBuilder routeHandlerBuilder)
    {
        public RouteHandlerBuilder UsesCursorPagingResponse()
        {
            return routeHandlerBuilder.AddEndpointFilter(async (context, next) =>
            {
                var filter = new ValidateCursorQueryInRangeFilter();
                return await filter.InvokeAsync(context, next);
            });
        }
    }
}