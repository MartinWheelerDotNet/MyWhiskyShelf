using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results.Distilleries;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.WebApi.Contracts.Common;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.Extensions;
using MyWhiskyShelf.WebApi.Mapping;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.WebApi.Endpoints;

[ExcludeFromCodeCoverage]
public static class DistilleryEndpoints
{
    private const string EndpointGroup = "distillery";

    public static void MapDistilleryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/distilleries")
            .WithTags("Distilleries")
            .WithOpenApi();

        group.MapPost(
                "/",
                async (
                    [FromBody] DistilleryCreateRequest request,
                    [FromServices] IDistilleryAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.CreateAsync(request.ToDomain(), ct);

                    return result.Outcome switch
                    {
                        CreateDistilleryOutcome.Created => Results.Created(
                            $"/distilleries/{result.Distillery!.Id}",
                            result.Distillery.ToResponse()),
                        CreateDistilleryOutcome.AlreadyExists => Results.Conflict(),
                        CreateDistilleryOutcome.CountryDoesNotExist =>
                            ValidationProblemResults.InvalidPagingParameters(
                                new Dictionary<string, string[]>
                                {
                                    { "countryId", ["Country does not exist."] }
                                }),
                        CreateDistilleryOutcome.RegionDoesNotExistInCountry =>
                            ValidationProblemResults.InvalidPagingParameters(
                                new Dictionary<string, string[]>
                                {
                                    { "regionId", ["Region does not exist in the specified country."] }
                                }),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "create",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Create Distillery")
            .Accepts<DistilleryCreateRequest>(MediaTypeNames.Application.Json)
            .Produces<DistilleryResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequiresIdempotencyKey()
            .RequireAuthorization(Policies.WriteDistilleries);

        group.MapGet(
                "/{id:guid}",
                async (
                    [FromRoute] Guid id,
                    [FromServices] IDistilleryAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.GetByIdAsync(id, ct);

                    return result.Outcome switch
                    {
                        GetDistilleryByIdOutcome.Success => Results.Ok(result.Distillery!.ToResponse()),
                        GetDistilleryByIdOutcome.NotFound => Results.NotFound(),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "get-by-id",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Get Distillery")
            .Produces<DistilleryResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id")
            .RequireAuthorization(Policies.ReadDistilleries);

        group.MapGet(
                "/",
                async (
                    [FromServices] IDistilleryAppService service,
                    HttpContext httpContext,
                    CancellationToken ct,
                    [FromQuery(Name = "cursor")] string? cursor = null,
                    [FromQuery(Name = "amount")] int amount = 10,
                    [FromQuery(Name = "countryId")] Guid? countryId = null,
                    [FromQuery(Name = "regionId")] Guid? regionId = null,
                    [FromQuery(Name = "pattern")] string? pattern = null) =>
                {
                    var result = !string.IsNullOrWhiteSpace(cursor)
                        ? await service.GetAllAsync(amount, cursor, ct)
                        : await service.GetAllAsync(
                            new DistilleryFilterOptions(countryId, regionId, pattern?.Trim(), amount),
                            ct);
                    
                    return result.Outcome switch
                    {
                        GetAllDistilleriesOutcome.Success => Results.Ok(
                            new CursorPagedResponse<DistilleryResponse>(
                                result.Distilleries!.Select(d => d.ToResponse()).ToList(),
                                result.NextCursor,
                                result.Amount
                            )
                        ),
                        GetAllDistilleriesOutcome.InvalidCursor => ValidationProblemResults.InvalidPagingParameters(
                            new Dictionary<string, string[]>
                            {
                                { "cursor", ["The provided 'cursor' is invalid or malformed."] }
                            }),
                        _ => ProblemResults.InternalServerError(
                            "distillery",
                            "get-all",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Get All Distilleries")
            .Produces<CursorPagedResponse<DistilleryResponse>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .UsesCursorPagingResponse()
            .RequireAuthorization(Policies.ReadDistilleries);

        group.MapPut(
                "/{id:guid}",
                async (
                    [FromRoute] Guid id,
                    [FromBody] DistilleryUpdateRequest request,
                    [FromServices] IDistilleryAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.UpdateAsync(id, request.ToDomain(id), ct);

                    return result.Outcome switch
                    {
                        UpdateDistilleryOutcome.Updated => Results.Ok(result.Distillery!.ToResponse()),
                        UpdateDistilleryOutcome.NotFound => Results.NotFound(),
                        UpdateDistilleryOutcome.NameConflict => Results.Conflict(),
                        UpdateDistilleryOutcome.CountryDoesNotExist =>
                            ValidationProblemResults.InvalidPagingParameters(
                                new Dictionary<string, string[]>
                                {
                                    { "countryId", ["Country does not exist."] }
                                }),
                        UpdateDistilleryOutcome.RegionDoesNotExistInCountry =>
                            ValidationProblemResults.InvalidPagingParameters(
                                new Dictionary<string, string[]>
                                {
                                    { "regionId", ["Region does not exist in the specified country."] }
                                }),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "update",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Update Distillery")
            .Produces<DistilleryResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id")
            .RequiresIdempotencyKey()
            .RequireAuthorization(Policies.WriteDistilleries);

        group.MapDelete(
                "/{id:guid}",
                async (
                    [FromRoute] Guid id,
                    [FromServices] IDistilleryAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.DeleteAsync(id, ct);

                    return result.Outcome switch
                    {
                        DeleteDistilleryOutcome.Deleted => Results.NoContent(),
                        DeleteDistilleryOutcome.NotFound => Results.NotFound(),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "delete",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Delete Distillery")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id")
            .RequiresIdempotencyKey()
            .RequireAuthorization(Policies.WriteDistilleries);
    }
}