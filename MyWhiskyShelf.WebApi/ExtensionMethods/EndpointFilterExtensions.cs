using MyWhiskyShelf.WebApi.Filters;

namespace MyWhiskyShelf.WebApi.ExtensionMethods;

public static class EndpointFilterExtensions
{
    public static RouteHandlerBuilder RequiresNonEmptyRouteParameter(
        this RouteHandlerBuilder routeHandlerBuilder,
        string parameterName)
    {
        return routeHandlerBuilder.AddEndpointFilter(new ValidateNonEmptyRouteParameterFilter(parameterName));
    }

    public static RouteHandlerBuilder RequiresNonEmptyQueryParameter(
        this RouteHandlerBuilder routeHandlerBuilder,
        string parameterName)
    {
        return routeHandlerBuilder.AddEndpointFilter(new ValidateNonEmptyQueryParameterFilter(parameterName));
    }
}