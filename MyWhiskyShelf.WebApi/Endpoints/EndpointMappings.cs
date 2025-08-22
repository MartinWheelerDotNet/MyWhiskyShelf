using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.ExtensionMethods;

namespace MyWhiskyShelf.WebApi.Endpoints;

[ExcludeFromCodeCoverage]
internal static class EndpointMappings
{
    private const string DistilleryWithRouteIdEndpoint = "/distilleries/{id:guid}";
    private const string DistilleriesEndpoint = "/distilleries";
    private const string GetAllDistilleryNameDetailsEndpoint = "/distilleries/names";
    private const string SearchForDetailsByNameEndpoint = "/distilleries/name/search";
    private const string WhiskyBottleEndpoint = "/whisky-bottle";
    private const string WhiskyBottleWithRouteIdEndpoint = "/whisky-bottle/{id:guid}";
    
    private const string DistilleriesTag = "Distilleries";
    private const string WhiskyBottleTag = "WhiskyBottle";
    private const string DistilleryNameDetailsTag = "DistilleryNameDetails";
    
    public static void MapDistilleryEndpoints(this WebApplication app)
    {
        app.MapPost(
                DistilleriesEndpoint,
                async (
                    [FromServices] IDistilleryWriteService distilleryWriteService,
                    [FromBody] DistilleryRequest distilleryRequest,
                    HttpContext httpContext) =>
                {
                    var (hasBeenAdded, id) = await distilleryWriteService.TryAddDistilleryAsync(distilleryRequest);

                    return hasBeenAdded
                        ? Results.Created($"{DistilleriesEndpoint}/{id}", null)
                        : ProblemResults.ResourceAlreadyExists(
                            "distillery",
                            distilleryRequest.Name,
                            httpContext);
                })
            .WithName("Create Distillery")
            .WithTags(DistilleriesTag)
            .RequiresIdempotencyKey()
            .Accepts<DistilleryRequest>(MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapGet(
                DistilleryWithRouteIdEndpoint,
                async ([FromServices] IDistilleryReadService distilleryReadService, [FromRoute] Guid id) =>
                {
                    var distillery = await distilleryReadService.GetDistilleryByIdAsync(id);
                    return distillery is null
                        ? Results.NotFound()
                        : Results.Ok(distillery);
                })
            .WithName("Get Distillery")
            .WithTags(DistilleriesTag)
            .RequiresNonEmptyRouteParameter("id")
            .Produces<DistilleryRequest>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                DistilleriesEndpoint,
                async ([FromServices] IDistilleryReadService distilleryReadService) =>
                {
                    var distilleries = await distilleryReadService.GetAllDistilleriesAsync();
                    return Results.Ok(distilleries);
                })
            .WithName("Get All Distilleries")
            .WithTags(DistilleriesTag)
            .Produces<List<DistilleryRequest>>();

        app.MapPut(
                DistilleryWithRouteIdEndpoint,
                async (
                    [FromServices] IDistilleryWriteService distilleryWriteService,
                    [FromRoute] Guid id,
                    [FromBody] DistilleryRequest request,
                    HttpContext httpContext) => 
                        await distilleryWriteService.TryUpdateDistilleryAsync(id, request) 
                            ? Results.NoContent()
                            : ProblemResults.ResourceNotFound("distillery", "update", id, httpContext))
            .WithName("Update Distillery")
            .WithTags(DistilleriesTag)
            .RequiresIdempotencyKey()
            .RequiresNonEmptyRouteParameter("id")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapDelete(
                DistilleryWithRouteIdEndpoint,
                async (
                    [FromServices] IDistilleryWriteService distilleryWriteService,
                    [FromRoute] Guid id) =>
                    {
                        await distilleryWriteService.RemoveDistilleryAsync(id); return Results.NoContent();
                    })
            .WithName("Delete Distillery")
            .WithTags(DistilleriesTag)
            .RequiresIdempotencyKey()
            .RequiresNonEmptyRouteParameter("id")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
    
    public static void MapDistilleryNameEndpoints(this WebApplication app)
    {
        app.MapGet(
                GetAllDistilleryNameDetailsEndpoint,
                ([FromServices] IDistilleryNameCacheService nameCacheService) =>
                    Results.Ok(nameCacheService.GetAll()))
            .WithName("Get All Distillery Name Details").Produces<List<DistilleryNameDetails>>()
            .WithTags(DistilleryNameDetailsTag)
            .Produces<List<DistilleryRequest>>();

        app.MapGet(
                SearchForDetailsByNameEndpoint,
                ([FromServices] IDistilleryNameCacheService nameCacheService, [FromQuery] string? pattern) =>
                    Results.Ok(nameCacheService.Search(pattern!)))
            .WithName("Search by Query Pattern")
            .WithTags(DistilleryNameDetailsTag)
            .RequiresNonEmptyQueryParameter("pattern")
            .Produces<List<DistilleryNameDetails>>()
            .ProducesValidationProblem();
    }
    
    public static void MapWhiskyBottleEndpoints(this WebApplication app)
    {
        app.MapGet(
                WhiskyBottleWithRouteIdEndpoint,
                async (
                    [FromServices] IWhiskyBottleReadService whiskyBottleReadService,
                    [FromRoute] Guid id) =>
                {
                    var whiskyBottle = await whiskyBottleReadService.GetByIdAsync(id);
                    return whiskyBottle is null
                        ? Results.NotFound()
                        : Results.Ok(whiskyBottle);
                })
            .WithName("Get Whisky Bottle")
            .WithTags(WhiskyBottleTag)
            .Produces<WhiskyBottleResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(
                WhiskyBottleEndpoint,
                async (
                    [FromServices] IWhiskyBottleWriteService whiskyBottleWriteService,
                    [FromBody] WhiskyBottleRequest whiskyBottleRequest) =>
                {
                    var (hasBeenAdded, id) = await whiskyBottleWriteService
                        .TryAddAsync(whiskyBottleRequest);

                    return hasBeenAdded
                        ? Results.Created($"{WhiskyBottleEndpoint}/{id}", null)
                        : ValidationProblemResults.WhiskyBottleValidationProblemResults();
                })
            .WithName("Add Whisky Bottle")
            .WithTags(WhiskyBottleTag)
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        app.MapPut(
                WhiskyBottleWithRouteIdEndpoint,
                async (
                    [FromServices] IWhiskyBottleWriteService whiskyBottleWriteService,
                    [FromRoute] Guid id,
                    [FromBody] WhiskyBottleRequest whiskyBottleRequest,
                    HttpContext httpContext) =>
                {
                    var hasBeenUpdated = await whiskyBottleWriteService.TryUpdateAsync(id, whiskyBottleRequest);

                    return hasBeenUpdated
                        ? Results.Ok()
                        : ProblemResults.ResourceNotFound("whisky-bottle", "update", id, httpContext);
                })
            .WithName("Update Whisky Bottle")
            .WithTags(WhiskyBottleTag)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapDelete(
            WhiskyBottleWithRouteIdEndpoint,
            async (
                [FromServices] IWhiskyBottleWriteService whiskyBottleWriteService,
                [FromRoute] Guid id,
                HttpContext httpContext) =>
            {
                var hasBeenDeleted = await whiskyBottleWriteService.TryDeleteAsync(id);

                return hasBeenDeleted
                    ? Results.Ok()
                    : ProblemResults.ResourceNotFound("whisky-bottle", "delete", id, httpContext);
            });
    }
}