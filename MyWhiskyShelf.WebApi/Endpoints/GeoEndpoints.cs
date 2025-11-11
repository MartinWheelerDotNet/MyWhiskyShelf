using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results.GeoData;
using MyWhiskyShelf.WebApi.Constants;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.Extensions;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Endpoints;

public static class GeoEndpoints
{
    private const string EndpointGroup = "geodata";
    private const string BaseUrl = "/geo";

    public static void MapGeoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BaseUrl)
            .WithTags("GeoData");

        group.MapGet(
                "/",
                async (
                    [FromServices] IGeoAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.GetAllAsync(ct);

                    return result.Outcome switch
                    {
                        GetCountryGeoOutcome.Success => Results.Ok(
                            result.Countries!.Select(c => c.ToResponse())),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "get-all",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Get All Geo Data")
            .Produces<List<CountryResponse>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(Authentication.Policies.ReadGeoData);

        group.MapPost(
                "/countries",
                async (
                    [FromBody] CountryCreateRequest request,
                    [FromServices] IGeoAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.CreateCountryAsync(request.ToDomain(), ct);

                    return result.Outcome switch
                    {
                        CreateCountryOutcome.Created => Results.Created("/geo", result.Country!.ToResponse()),
                        CreateCountryOutcome.NameConflict => Results.Conflict(),
                        _ => ProblemResults.InternalServerError(
                            "country",
                            "create",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Create Country")
            .Accepts<CountryCreateRequest>(MediaTypeNames.Application.Json)
            .Produces<CountryResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequiresIdempotencyKey()
            .RequireAuthorization(Authentication.Policies.WriteGeoData);

        group.MapPost(
                "/regions",
                async (
                    [FromBody] RegionCreateRequest request,
                    [FromServices] IGeoAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.CreateRegionAsync(request.CountryId, request.ToDomain(), ct);

                    return result.Outcome switch
                    {
                        CreateRegionOutcome.Created => Results.Created("/geo", result.Region!.ToResponse()),
                        CreateRegionOutcome.NameConflict => Results.Conflict(),
                        CreateRegionOutcome.CountryNotFound => ValidationProblemResults.CountryNotFound(
                            request.CountryId),
                        _ => ProblemResults.InternalServerError(
                            "region",
                            "create",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Create Region")
            .Accepts<RegionCreateRequest>(MediaTypeNames.Application.Json)
            .Produces<RegionResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequiresIdempotencyKey()
            .RequireAuthorization(Authentication.Policies.WriteGeoData);
    }
}