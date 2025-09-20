using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;
using MyWhiskyShelf.WebApi.ErrorResults;
using MyWhiskyShelf.WebApi.ExtensionMethods;
using MyWhiskyShelf.WebApi.Mapping;

namespace MyWhiskyShelf.WebApi.Endpoints;

// Endpoints are covered by integration tests which are managed by Aspire, and do not need to be tested independently
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
                        _ => Results.Problem(ProblemResults.InternalServerError(
                            EndpointGroup,
                            "create",
                            httpContext.TraceIdentifier,
                            httpContext.Request.Path))
                    };
                })
            .WithName("Create Distillery")
            .Accepts<DistilleryCreateRequest>(MediaTypeNames.Application.Json)
            .Produces<DistilleryResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresIdempotencyKey();

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
                        _ => Results.Problem(
                            ProblemResults.InternalServerError(
                                EndpointGroup,
                                "get-by-id",
                                httpContext.TraceIdentifier,
                                httpContext.Request.Path))
                    };

                })
            .WithName("Get Distillery")
            .Produces<DistilleryResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id");

        group.MapGet(
                "/",
                async (
                    [FromServices] IDistilleryAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.GetAllAsync(ct);

                    return result.Outcome switch
                    {
                        GetAllDistilleriesOutcome.Success => Results.Ok(
                            result.Distilleries!.Select(distillery => distillery.ToResponse())),
                        _ => Results.Problem(
                            ProblemResults.InternalServerError(
                                EndpointGroup,
                                "get-all",
                                httpContext.TraceIdentifier,
                                httpContext.Request.Path))
                    };
                })
            .WithName("Get All Distilleries")
            .Produces<List<DistilleryResponse>>();

        group.MapGet(
                "/search",
                async (
                    [FromQuery] string pattern,
                    [FromServices] IDistilleryAppService service,
                    HttpContext httpContext,
                    CancellationToken ct) =>
                {
                    var result = await service.SearchByNameAsync(pattern, ct);

                    return result.Outcome switch
                    {
                        SearchDistilleriesOutcome.Success => Results.Ok(
                            result.DistilleryNames!.Select(distillery => distillery.ToResponse())),
                        _ => Results.Problem(
                            ProblemResults.InternalServerError(
                                EndpointGroup,
                                "search",
                                httpContext.TraceIdentifier,
                                httpContext.Request.Path))

                    };
                })
            .WithName("Search Distilleries")
            .Produces<List<DistilleryResponse>>()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyQueryParameter("pattern");

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
                        _ => Results.Problem(
                            ProblemResults.InternalServerError(
                                EndpointGroup,
                                "update",
                                httpContext.TraceIdentifier,
                                httpContext.Request.Path))
                    };
                })
            .WithName("Update Distillery")
            .Produces<DistilleryResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id")
            .RequiresIdempotencyKey();
        
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
                        _ => Results.Problem(
                            ProblemResults.InternalServerError(
                                EndpointGroup,
                                "delete",
                                httpContext.TraceIdentifier,
                                httpContext.Request.Path))
                    };
                })
            .WithName("Delete Distillery")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequiresNonEmptyRouteParameter("id")
            .RequiresIdempotencyKey();
    }
}