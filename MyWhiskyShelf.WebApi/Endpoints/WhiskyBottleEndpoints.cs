using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ErrorResults;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static partial class EndpointMappings
{
    private const string WhiskyBottleEndpoint = "/whisky-bottle";
    private const string WhiskyBottleWithRouteIdentifierEndpoint = "/whisky-bottle/{identifier:guid}";
    private const string WhiskyBottleTag = "WhiskyBottle";

    public static void MapWhiskyBottleEndpoints(this WebApplication app)
    {
        app.MapGet(
                WhiskyBottleWithRouteIdentifierEndpoint,
                async (
                    [FromServices] IWhiskyBottleReadService whiskyBottleReadService,
                    [FromRoute] Guid identifier) =>
                {
                    var whiskyBottle = await whiskyBottleReadService.GetByIdAsync(identifier);
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
                    var (hasBeenAdded, identifier) = await whiskyBottleWriteService
                        .TryAddAsync(whiskyBottleRequest);

                    return hasBeenAdded
                        ? Results.Created($"{WhiskyBottleEndpoint}/{identifier}", null)
                        : ValidationProblemResults.WhiskyBottleValidationProblemResults();
                })
            .WithName("Add Whisky Bottle")
            .WithTags(WhiskyBottleTag)
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        app.MapDelete(
            WhiskyBottleWithRouteIdentifierEndpoint,
            async (
                [FromServices] IWhiskyBottleWriteService whiskyBottleWriteService,
                [FromRoute] Guid identifier,
                HttpContext httpContext) =>
            {
                var hasBeenDeleted = await whiskyBottleWriteService.TryDeleteAsync(identifier);

                return hasBeenDeleted
                    ? Results.Ok()
                    : ProblemResults.ResourceNotFound("whisky-bottle", identifier, httpContext);
            });
    }
    
}