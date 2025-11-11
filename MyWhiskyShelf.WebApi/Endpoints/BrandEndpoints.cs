using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results.Brands;
using MyWhiskyShelf.WebApi.Contracts.Brands;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.Mapping;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.WebApi.Endpoints;

[ExcludeFromCodeCoverage]
public static class BrandEndpoints
{
    private const string EndpointGroup = "brand";

    public static void MapBrandEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/brands")
            .WithTags("Brands");

        group.MapGet(
                "/",
                async (
                    [FromServices] IBrandAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.GetBrandsAsync(ct);

                    return result.Outcome switch
                    {
                        GetBrandsOutcome.Success => Results.Ok(
                            result.Brands!.Select(brand => brand.ToResponse()).ToList()),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "get-all",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Get Brands")
            .Produces<BrandResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization(Policies.ReadBrands);
    }
}