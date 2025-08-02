using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ExtensionMethods;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static class EndpointMappings
{
    #region [/distilleries] - Distillery Endpoint Mappings

    public static void MapDistilleryEndpoints(this WebApplication app)
    {
        app.MapGet(
                "/distilleries/{distilleryName}",
                async ([FromServices] IDistilleryReadService distilleryReadService, string distilleryName) =>
                {
                    var distillery = await distilleryReadService.GetDistilleryByNameAsync(distilleryName);
                    return distillery is null
                        ? Results.NotFound()
                        : Results.Ok(distillery);
                })
            .WithName("Get DistilleryRequest By Name")
            .WithTags("Distilleries")
            .RequiresNonEmptyRouteParameter("distilleryName")
            .Produces<DistilleryRequest>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                "/distilleries",
                async ([FromServices] IDistilleryReadService distilleryReadService) =>
                {
                    var distilleries = await distilleryReadService.GetAllDistilleriesAsync();
                    return Results.Ok(distilleries);
                })
            .WithName("Get All Distilleries")
            .WithTags("Distilleries")
            .Produces<List<DistilleryRequest>>();

        app.MapGet(
                "/distilleries/names",
                ([FromServices] IDistilleryReadService distilleryReadService) =>
                    Results.Ok(distilleryReadService.GetDistilleryNames()))
            .WithName("Get All DistilleryRequest Name Details").Produces<List<DistilleryNameDetails>>()
            .WithTags("Distilleries")
            .Produces<List<DistilleryRequest>>();

        app.MapGet(
                "/distilleries/name/search",
                ([FromServices] IDistilleryReadService distilleryReadService, [FromQuery] string? pattern) =>
                    Results.Ok(distilleryReadService.SearchByName(pattern!)))
            .WithName("Search by Query Pattern")
            .WithTags("Distilleries")
            .RequiresNonEmptyQueryParameter("pattern")
            .Produces<List<DistilleryNameDetails>>()
            .ProducesValidationProblem();

        app.MapPost(
                "/distilleries/add",
                async (
                        [FromServices] IDistilleryWriteService distilleryWriteService,
                        [FromBody] DistilleryRequest distilleryRequest,
                        HttpContext httpContext) =>
                    await distilleryWriteService.TryAddDistilleryAsync(distilleryRequest)
                        ? Results.Created($"/distilleries/{Uri.EscapeDataString(distilleryRequest.DistilleryName)}",
                            null)
                        : ProblemResults.DistilleryAlreadyExists(distilleryRequest.DistilleryName, httpContext))
            .WithName("Add DistilleryRequest")
            .WithTags("Distilleries")
            .Accepts<DistilleryRequest>(MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapDelete(
                "/distilleries/remove/{distilleryName}",
                async (
                        [FromServices] IDistilleryWriteService distilleryWriteService,
                        [FromRoute] string distilleryName,
                        HttpContext httpContext) =>
                    await distilleryWriteService.TryRemoveDistilleryAsync(Uri.UnescapeDataString(distilleryName))
                        ? Results.Ok()
                        : ProblemResults.DistilleryNotFound(distilleryName, httpContext))
            .WithName("Remove DistilleryRequest")
            .WithTags("Distilleries")
            .RequiresNonEmptyRouteParameter("distilleryName")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    #endregion

    #region [/whisky-bottle] - Whisky Bottle Endpoint Mappings

    public static void MapWhiskyBottleEndpoints(this WebApplication app)
    {
        app.MapGet(
                "/whisky-bottle/{identifier:guid}",
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
            .WithTags("WhiskyBottle")
            .Produces<WhiskyBottleResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(
                "/whisky-bottle/add",
                async (
                    [FromServices] IWhiskyBottleWriteService whiskyBottleWriteService,
                    [FromBody] WhiskyBottleRequest whiskyBottleRequest) =>
                {
                    var (hasBeenAdded, identifier) = await whiskyBottleWriteService
                        .TryAddAsync(whiskyBottleRequest);

                    return hasBeenAdded
                        ? Results.Created($"/whisky-bottle/{identifier}", null)
                        : ValidationProblemResults.WhiskyBottleValidationProblemResults();
                })
            .WithName("Add Whisky Bottle")
            .WithTags("WhiskyBottle")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    #endregion
}