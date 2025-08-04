using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.ExtensionMethods;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static partial class EndpointMappings
{
    private const string DistilleryWithRouteIdentifierEndpoint = "/distilleries/{identifier:guid}";
    private const string DistilleriesEndpoint = "/distilleries";
    private const string DistilleriesTag = "Distilleries";

    public static void MapDistilleryEndpoints(this WebApplication app)
    {
        app.MapPost(
                DistilleriesEndpoint,
                async (
                    [FromServices] IDistilleryWriteService distilleryWriteService,
                    [FromBody] CreateDistilleryRequest createDistilleryRequest,
                    HttpContext httpContext) =>
                {
                    var (hasBeenAdded, identifier) =
                        await distilleryWriteService.TryAddDistilleryAsync(createDistilleryRequest);

                    return hasBeenAdded
                        ? Results.Created($"{DistilleriesEndpoint}/{identifier}", null)
                        : ProblemResults.ResourceAlreadyExists(
                            "distillery",
                            createDistilleryRequest.DistilleryName,
                            httpContext);
                })
            .WithName("Create Distillery")
            .WithTags(DistilleriesTag)
            .Accepts<CreateDistilleryRequest>(MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapGet(
                DistilleryWithRouteIdentifierEndpoint,
                async ([FromServices] IDistilleryReadService distilleryReadService,
                    [FromRoute] Guid identifier) =>
                {
                    var distillery = await distilleryReadService.GetDistilleryByIdAsync(identifier);
                    return distillery is null
                        ? Results.NotFound()
                        : Results.Ok(distillery);
                })
            .WithName("Get Distillery")
            .WithTags(DistilleriesTag)
            .RequiresNonEmptyRouteParameter("identifier")
            .Produces<CreateDistilleryRequest>(StatusCodes.Status201Created)
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
            .Produces<List<CreateDistilleryRequest>>();

        app.MapDelete(
                DistilleryWithRouteIdentifierEndpoint,
                async (
                        [FromServices] IDistilleryWriteService distilleryWriteService,
                        [FromRoute] Guid identifier,
                        HttpContext httpContext) =>
                    await distilleryWriteService.TryRemoveDistilleryAsync(identifier)
                        ? Results.Ok()
                        : ProblemResults.ResourceNotFound("distillery", identifier, httpContext))
            .WithName("Delete Distillery")
            .WithTags(DistilleriesTag)
            .RequiresNonEmptyRouteParameter("identifier")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}