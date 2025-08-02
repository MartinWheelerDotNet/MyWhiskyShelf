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
                ([FromServices] IDistilleryNameCacheService nameCacheService) =>
                    Results.Ok(nameCacheService.GetAll()))
            .WithName("Get All DistilleryRequest Name Details").Produces<List<DistilleryNameDetails>>()
            .WithTags(DistilleryNameDetailsTag)
            .Produces<List<DistilleryRequest>>();

        app.MapGet(
                SearchForDetailsByNameEndpoint,
                ([FromServices] IDistilleryNameCacheService nameCacheService, [FromQuery] string? pattern) =>
                    Results.Ok(nameCacheService.Search(pattern!)))
            .WithName("Search by Query Pattern")
            .WithTags(DistilleryNameDetailsTag)
            .RequiresNonEmptyQueryParameter("pattern")
            .Produces<List<DistilleryNameDetails>>()
            .ProducesValidationProblem();
    }
}