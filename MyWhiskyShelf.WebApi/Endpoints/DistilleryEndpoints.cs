using System.Diagnostics.CodeAnalysis;
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
}