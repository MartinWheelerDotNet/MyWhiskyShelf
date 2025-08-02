using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ExtensionMethods;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static partial class EndpointMappings
{
    private const string GetDistilleryByIdEndpoint = "/distilleries/{identifier:guid}";
    private const string DistilleriesEndpoint = "/distilleries";
    private const string DistilleriesTag = "Distilleries";

    public static void MapDistilleryEndpoints(this WebApplication app)
    {
        app.MapGet(
                GetDistilleryByIdEndpoint,
                async ([FromServices] IDistilleryReadService distilleryReadService, 
                    [FromRoute] Guid identifier) =>
                {
                    var distillery = await distilleryReadService.GetDistilleryByIdAsync(identifier);
                    return distillery is null
                        ? Results.NotFound()
                        : Results.Ok(distillery);
                })
            .WithName("Get DistilleryRequest By Name")
            .WithTags(DistilleriesTag)
            .RequiresNonEmptyRouteParameter("identifier")
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
        
        app.MapPost(
                DistilleriesEndpoint,
                async (
                        [FromServices] IDistilleryWriteService distilleryWriteService,
                        [FromBody] DistilleryRequest distilleryRequest,
                        HttpContext httpContext) =>
                    await distilleryWriteService.TryAddDistilleryAsync(distilleryRequest)
                        ? Results.Created($"{DistilleriesEndpoint}/{Uri.EscapeDataString(distilleryRequest.DistilleryName)}",
                            null)
                        : ProblemResults.ProblemResults.DistilleryAlreadyExists(distilleryRequest.DistilleryName, httpContext))
            .WithName("Add DistilleryRequest")
            .WithTags(DistilleriesTag)
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
                        : ProblemResults.ProblemResults.DistilleryNotFound(distilleryName, httpContext))
            .WithName("Remove DistilleryRequest")
            .WithTags(DistilleriesTag)
            .RequiresNonEmptyRouteParameter("distilleryName")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}