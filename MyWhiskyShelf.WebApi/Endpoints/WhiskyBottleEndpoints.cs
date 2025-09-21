using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.Extensions;
using MyWhiskyShelf.WebApi.Mapping;
using static MyWhiskyShelf.WebApi.Constants.Authentication;

namespace MyWhiskyShelf.WebApi.Endpoints;

// Endpoints are covered by integration tests which are managed by Aspire, and do not need to be tested independently
[ExcludeFromCodeCoverage]
public static class WhiskyBottleEndpoints
{
    private const string EndpointGroup = "whisky-bottle";
    
    public static void MapWhiskyBottleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/whisky-bottles")
            .WithTags("Whisky Bottles")
            .WithOpenApi();

        group.MapPost(
                "/",
                async (
                    [FromServices] IWhiskyBottleAppService service,
                    [FromBody] WhiskyBottleCreateRequest request,
                    HttpContext httpContext) =>
                {
                    var result = await service.CreateAsync(request.ToDomain());

                    return result.Outcome switch
                    {
                        CreateWhiskyBottleOutcome.Created => Results.Created(
                            $"/whisky-bottles/{result.WhiskyBottle!.Id}",
                            result.WhiskyBottle!.ToResponse()),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "create",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Add Whisky Bottle")
            .Accepts<WhiskyBottleCreateRequest>(MediaTypeNames.Application.Json)
            .Produces<WhiskyBottleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresIdempotencyKey()
            .RequireAuthorization(Policies.WriteWhiskyBottles);

        group.MapGet(
                "/{id:guid}", async (
                    [FromServices] IWhiskyBottleAppService service,
                    HttpContext httpContext,
                    [FromRoute] Guid id) =>
                {
                    var result = await service.GetByIdAsync(id);

                    return result.Outcome switch
                    {
                        GetWhiskyBottleByIdOutcome.Success => Results.Ok(result.WhiskyBottle!.ToResponse()),
                        GetWhiskyBottleByIdOutcome.NotFound => Results.NotFound(),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "get-by-id",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };

                })
            .WithName("Get Whisky Bottle")
            .Produces<WhiskyBottleResponse>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id")
            .RequireAuthorization(Policies.ReadWhiskyBottles);

        group.MapPut(
                "/{id:guid}",
                async (
                    [FromServices] IWhiskyBottleAppService service,
                    [FromRoute] Guid id,
                    [FromBody] WhiskyBottleUpdateRequest request,
                    HttpContext httpContext) =>
                {
                    var result = await service.UpdateAsync(id, request.ToDomain());

                    return result.Outcome switch
                    {
                        UpdateWhiskyBottleOutcome.Updated => Results.Ok(result.WhiskyBottle!.ToResponse()),
                        UpdateWhiskyBottleOutcome.NotFound => Results.NotFound(),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "update",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Update Whisky Bottle")
            .Accepts<WhiskyBottleUpdateRequest>(MediaTypeNames.Application.Json)
            .Produces<WhiskyBottleResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequiresNonEmptyRouteParameter("id")
            .RequiresIdempotencyKey()
            .RequireAuthorization(Policies.WriteWhiskyBottles);

        group.MapDelete(
                "/{id:guid}",
                async (
                    [FromServices] IWhiskyBottleAppService service,
                    [FromRoute] Guid id,
                    HttpContext httpContext) =>
                {
                    var result = await service.DeleteAsync(id);

                    return result.Outcome switch
                    {
                        DeleteWhiskyBottleOutcome.Deleted => Results.NoContent(),
                        DeleteWhiskyBottleOutcome.NotFound => Results.NotFound(),
                        _ => ProblemResults.InternalServerError(
                            EndpointGroup,
                            "delete",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path)
                    };
                })
            .WithName("Delete Whisky Bottle")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id")
            .RequiresIdempotencyKey()
            .RequireAuthorization(Policies.WriteWhiskyBottles);
    }
}