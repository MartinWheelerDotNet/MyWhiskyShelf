using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ErrorResults;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static partial class EndpointMappings
{
    private const string WhiskyBottleEndpoint = "/whisky-bottle";
    private const string WhiskyBottleWithRouteIdEndpoint = "/whisky-bottle/{id:guid}";
    private const string WhiskyBottleTag = "WhiskyBottle";

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