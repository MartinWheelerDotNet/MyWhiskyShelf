using System.Net.Mime;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ExtensionMethods;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static class EndpointMappings
{
    #region Distillery Endpoint Mappings

    public static void MapDistilleryEndpoints(this WebApplication app)
    {
        app.MapGet(
                "/distilleries/{distilleryName}",
                async (IDistilleryReadService distilleryReadService, string distilleryName) =>
                {
                    var distillery = await distilleryReadService.GetDistilleryByNameAsync(distilleryName);
                    return distillery is null
                        ? Results.NotFound()
                        : Results.Ok(distillery);
                })
            .WithName("Get Distillery By Name")
            .WithTags("Distilleries")
            .RequiresNonEmptyRouteParameter("distilleryName")
            .Produces<Distillery>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(
                "/distilleries",
                async (IDistilleryReadService distilleryReadService) =>
                {
                    var distilleries = await distilleryReadService.GetAllDistilleriesAsync();
                    return Results.Ok(distilleries);
                })
            .WithName("Get All Distilleries")
            .WithTags("Distilleries")
            .Produces<List<Distillery>>();

        app.MapGet(
                "/distilleries/names",
                (IDistilleryReadService distilleryReadService)
                    => Results.Ok(distilleryReadService.GetDistilleryNames()))
            .WithName("Get All Distillery Name Details").Produces<List<DistilleryNameDetails>>()
            .WithTags("Distilleries")
            .Produces<List<Distillery>>();

        app.MapGet(
                "/distilleries/name/search",
                (IDistilleryReadService distilleryReadService, string? pattern)
                    => Results.Ok(distilleryReadService.SearchByName(pattern!)))
            .WithName("Search by Query Pattern")
            .WithTags("Distilleries")
            .RequiresNonEmptyQueryParameter("pattern")
            .Produces<List<DistilleryNameDetails>>()
            .ProducesValidationProblem();

        app.MapPost(
                "/distilleries/add",
                async (IDistilleryWriteService distilleryWriteService, Distillery distillery, HttpContext httpContext)
                    => await distilleryWriteService.TryAddDistilleryAsync(distillery)
                        ? Results.Created($"/distilleries/{Uri.EscapeDataString(distillery.DistilleryName)}", null)
                        : ProblemResults.DistilleryAlreadyExists(distillery.DistilleryName, httpContext))
            .WithName("Add Distillery")
            .WithTags("Distilleries")
            .Accepts<Distillery>(MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status409Conflict);

        app.MapDelete(
                "/distilleries/remove/{distilleryName}",
                async (IDistilleryWriteService distilleryWriteService, string distilleryName, HttpContext httpContext)
                    => await distilleryWriteService.TryRemoveDistilleryAsync(Uri.UnescapeDataString(distilleryName))
                        ? Results.Ok()
                        : ProblemResults.DistilleryNotFound(distilleryName, httpContext))
            .WithName("Remove Distillery")
            .WithTags("Distilleries")
            .RequiresNonEmptyRouteParameter("distilleryName")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    #endregion

    #region WhiskyBottle Endpoint Mappings

    public static void MapWhiskyBottleEndpoints(this WebApplication app)
    {
        app.MapPost(
                "/whisky-bottle/add",
                async (WhiskyBottle whiskyBottle, IWhiskyBottleWriteService whiskyBottleWriteService)
                    => await whiskyBottleWriteService.TryAddAsync(whiskyBottle)
                        ? Results.Created($"/whisky-bottle/{Uri.EscapeDataString(whiskyBottle.Name)}", null)
                        : ValidationProblemResults.WhiskyBottleValidationProblemResults())
            .WithName("Add Whisky Bottle")
            .WithTags("WhiskyBottle")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    #endregion
}