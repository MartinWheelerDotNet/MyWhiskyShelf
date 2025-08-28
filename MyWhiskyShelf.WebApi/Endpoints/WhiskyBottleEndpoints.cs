using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.Application.Services;
using MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.ExtensionMethods;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Endpoints;

public static class WhiskyBottleEndpoints
{
    public static void MapWhiskyBottleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/whisky-bottles")
            .WithTags("Whisky Bottles")
            .WithOpenApi();

        group.MapPost(
                pattern: "/",
                handler: async (
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
                        _ => Results.Problem(ProblemResults.InternalServerError(
                            "whisky-bottle",
                            "create",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path))
                    };
                })
            .WithName("Add Whisky Bottle")
            .Accepts<WhiskyBottleCreateRequest>(MediaTypeNames.Application.Json)
            .Produces<WhiskyBottleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresIdempotencyKey();
        
        group.MapGet(
                pattern: "/{id:guid}", handler: async (
                     [FromServices] IWhiskyBottleAppService service,
                     [FromRoute] Guid id) =>
                 {
                     var whiskyBottle = await service.GetByIdAsync(id);
                     
                     return whiskyBottle is null
                         ? Results.NotFound()
                         : Results.Ok(whiskyBottle.ToResponse());
                 })
             .WithName("Get Whisky Bottle")
             .Produces<WhiskyBottleResponse>()
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesValidationProblem()
             .RequiresNonEmptyRouteParameter("id");

        group.MapPut(
                pattern: "/{id:guid}", 
                handler: async (
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
                        _ => Results.Problem(ProblemResults.InternalServerError(
                            "whisky-bottle",
                            "update",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path))
                    };
                })
            .WithName("Update Whisky Bottle")
            .Accepts<WhiskyBottleUpdateRequest>(MediaTypeNames.Application.Json)
            .Produces<WhiskyBottleResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequiresNonEmptyRouteParameter("id")
            .RequiresIdempotencyKey();

         group.MapDelete(
             pattern: "/{id:guid}", 
             handler: async (
                 [FromServices] IWhiskyBottleAppService service,
                 [FromRoute] Guid id,
                 HttpContext httpContext) =>
             {
                 var result = await service.DeleteAsync(id);

                 return result.Outcome switch
                 {
                     DeleteWhiskyBottleOutcome.Deleted => Results.NoContent(),
                     DeleteWhiskyBottleOutcome.NotFound => Results.NotFound(),
                     _ => Results.Problem(ProblemResults.InternalServerError(
                         "whisky-bottle",
                         "delete",
                         httpContext.TraceIdentifier,
                         httpContext.Request.Path))
                 };
             })
             .WithName("Delete Whisky Bottle")
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status500InternalServerError)
             .ProducesValidationProblem()
             .RequiresNonEmptyRouteParameter("id")
             .RequiresIdempotencyKey();
    }
}