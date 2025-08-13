using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.ExtensionMethods;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static partial class EndpointMappings
{
    private const string DistilleryWithRouteIdEndpoint = "/distilleries/{id:guid}";
    private const string DistilleriesEndpoint = "/distilleries";
    private const string DistilleriesTag = "Distilleries";

    public static void MapDistilleryEndpoints(this WebApplication app)
    {
        app.MapPost(
                DistilleriesEndpoint,
                async (
                    [FromServices] IDistilleryWriteService distilleryWriteService,
                    [FromBody] DistilleryRequest distilleryRequest,
                    [FromHeader] Guid? idempotencyKey,
                    HttpContext httpContext) =>
                {
                    if (idempotencyKey.HasValue)
                        return ValidationProblemResults.MissingIdempotencyKey();
                    
                    var (hasBeenAdded, id) =
                        await distilleryWriteService.TryAddDistilleryAsync(distilleryRequest, idempotencyKey!.Value);

                    return hasBeenAdded
                        ? Results.Created($"{DistilleriesEndpoint}/{id}", null)
                        : ProblemResults.ResourceAlreadyExists(
                            "distillery",
                            distilleryRequest.Name,
                            httpContext);
                })
            .WithName("Create Distillery")
            .WithTags(DistilleriesTag)
            .Accepts<DistilleryRequest>(MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status201Created)
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

        app.MapDelete(
                DistilleryWithRouteIdEndpoint,
                async (
                        [FromServices] IDistilleryWriteService distilleryWriteService,
                        [FromRoute] Guid id,
                        HttpContext httpContext) =>
                    await distilleryWriteService.TryRemoveDistilleryAsync(id)
                        ? Results.Ok()
                        : ProblemResults.ResourceNotFound("distillery", "delete", id, httpContext))
            .WithName("Delete Distillery")
            .WithTags(DistilleriesTag)
            .RequiresNonEmptyRouteParameter("id")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}