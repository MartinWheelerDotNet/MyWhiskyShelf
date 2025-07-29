using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
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
            .RequiresNonEmptyRouteParameter("distilleryName")
            .Produces<Distillery>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Distilleries");

        app.MapGet(
                "/distilleries",
                async (IDistilleryReadService distilleryReadService) =>
                {
                    var distilleries = await distilleryReadService.GetAllDistilleriesAsync();
                    return Results.Ok(distilleries);
                })
            .WithName("Get All Distilleries")
            .Produces<List<Distillery>>()
            .WithTags("Distilleries");

        app.MapGet(
                "/distilleries/names",
                (IDistilleryReadService distilleryReadService)
                    => Results.Ok(distilleryReadService.GetDistilleryNames()))
            .WithName("Get All Distillery Name Details").Produces<List<DistilleryNameDetails>>()
            .WithTags("Distilleries");

        app.MapGet(
                "/distilleries/name/search",
                (IDistilleryReadService distilleryReadService, string? pattern)
                    => Results.Ok(distilleryReadService.SearchByName(pattern!)))
            .WithName("Search by Query Pattern")
            .RequiresNonEmptyQueryParameter("pattern")
            .Produces<List<DistilleryNameDetails>>()
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        app.MapPost(
                "/distilleries/add",
                async (IDistilleryWriteService distilleryWriteService, Distillery distillery, HttpContext httpContext)
                    => await distilleryWriteService.TryAddDistilleryAsync(distillery)
                        ? Results.Created($"/distilleries/{Uri.EscapeDataString(distillery.DistilleryName)}", null)
                        : ProblemResults.DistilleryAlreadyExists(distillery.DistilleryName, httpContext))
            .WithName("Add Distillery")
            .Accepts<Distillery>(MediaTypeNames.Application.Json)
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .WithTags("Distilleries");

        app.MapDelete(
                "/distilleries/remove/{distilleryName}",
                async (IDistilleryWriteService distilleryWriteService, string distilleryName, HttpContext httpContext)
                    => await distilleryWriteService.TryRemoveDistilleryAsync(Uri.UnescapeDataString(distilleryName))
                        ? Results.Ok()
                        : ProblemResults.DistilleryNotFound(distilleryName, httpContext))
            .WithName("Remove Distillery")
            .RequiresNonEmptyRouteParameter("distilleryName")
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithTags("Distilleries");
    }

    #endregion

    #region WhiskyBottle Endpoint Mappings

    public static void MapWhiskyBottleEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/whiskyBottle/add",
            async (
                WhiskyBottle whiskyBottle,
                IWhiskyBottleWriteService whiskyBottleWriteService) =>
            {
                if (await whiskyBottleWriteService.TryAddAsync(whiskyBottle))
                    return Results.Created($"/whiskyBottle/{Uri.EscapeDataString(whiskyBottle.Name)}", null);

                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(WhiskyBottle)] =
                    [
                        """
                        An error occurred trying to add the whisky bottle to the database.
                        Ensure all required fields have been set.
                        """
                    ]
                });
            });
    }

    #endregion
}