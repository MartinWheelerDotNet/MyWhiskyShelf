using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.WebApi.ExtensionMethods;

namespace MyWhiskyShelf.WebApi.Endpoints;

internal static partial class EndpointMappings
{
    private const string GetAllDistilleryNameDetailsEndpoint = "/distilleries/names";
    private const string SearchForDetailsByNameEndpoint = "/distilleries/name/search";
    private const string DistilleryNameDetailsTag = "DistilleryNameDetails";

    public static void MapDistilleryNameEndpoints(this WebApplication app)
    {
        app.MapGet(
                GetAllDistilleryNameDetailsEndpoint,
                ([FromServices] IDistilleryReadService distilleryReadService) =>
                    Results.Ok(distilleryReadService.GetDistilleryNames()))
            .WithName("Get All DistilleryRequest Name Details").Produces<List<DistilleryNameDetails>>()
            .WithTags(DistilleryNameDetailsTag)
            .Produces<List<DistilleryRequest>>();

        app.MapGet(
                SearchForDetailsByNameEndpoint,
                ([FromServices] IDistilleryReadService distilleryReadService, [FromQuery] string? pattern) =>
                    Results.Ok(distilleryReadService.SearchByName(pattern!)))
            .WithName("Search by Query Pattern")
            .WithTags(DistilleryNameDetailsTag)
            .RequiresNonEmptyQueryParameter("pattern")
            .Produces<List<DistilleryNameDetails>>()
            .ProducesValidationProblem();
    }
}