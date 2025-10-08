using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Services;

public sealed class DistilleryAppService(
    IDistilleryReadRepository read,
    IDistilleryWriteRepository write,
    ILogger<DistilleryAppService> logger)
    : IDistilleryAppService
{
    public async Task<GetDistilleryByIdResult> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var distillery = await read.GetByIdAsync(id, ct);

            if (distillery is null)
            {
                logger.LogWarning("Distillery not found with [Id: {Id}]", id);
                return new GetDistilleryByIdResult(GetDistilleryByIdOutcome.NotFound);
            }

            logger.LogDebug("Retrieved distillery with [Name: {Name}, Id: {Id}]", distillery.Name.SanitizeForLog(), id);
            return new  GetDistilleryByIdResult(GetDistilleryByIdOutcome.Success, distillery);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving distillery with [Id: {Id}]", id);
            return new GetDistilleryByIdResult(GetDistilleryByIdOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<GetAllDistilleriesResult> GetAllAsync(int page, int amount, CancellationToken ct = default)
    {
        try
        {
            var distilleries = await read.GetAllAsync(page, amount, ct);
            
            logger.LogDebug("Retrieved [{Count}] distilleries (page {Page} / amount {Amount})",
                distilleries.Count,
                page,
                amount);

            return new GetAllDistilleriesResult(
                Outcome: GetAllDistilleriesOutcome.Success,
                Distilleries: distilleries,
                Page: page,
                Amount: amount
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured whilst retrieving all distilleries");
            return new GetAllDistilleriesResult(GetAllDistilleriesOutcome.Error, Error: ex.Message); 
        }
    }

    public async Task<SearchDistilleriesResult> SearchByNameAsync(string pattern, CancellationToken ct = default)
    {
        try
        {
            var distilleries = await read.SearchByNameAsync(pattern, ct);

            logger.LogDebug("Retrieved [{Count}] distilleries", distilleries.Count);
            return new SearchDistilleriesResult(SearchDistilleriesOutcome.Success, distilleries);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occured whilst searching for distilleries with [Pattern: {Pattern}",
                pattern.SanitizeForLog());
            return new SearchDistilleriesResult(SearchDistilleriesOutcome.Error, Error: ex.Message);
        }
        
    }
    
    public async Task<CreateDistilleryResult> CreateAsync(Distillery distillery, CancellationToken ct = default)
    {
        try
        {
            var exists = await read.ExistsByNameAsync(distillery.Name, ct);
            if (exists)
            {
                logger.LogWarning("Distillery already exists with [Name: {Name}]", distillery.Name.SanitizeForLog());
                return new CreateDistilleryResult(CreateDistilleryOutcome.AlreadyExists);
            }
            
            var addedDistillery = await write.AddAsync(distillery, ct);

            logger.LogDebug(
                "Distillery created with [Name: {Name}, Id: {Id}]",
                addedDistillery!.Name.SanitizeForLog(),
                addedDistillery.Id);
            
           return new CreateDistilleryResult(CreateDistilleryOutcome.Created, addedDistillery);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating distillery with [Name: {Name}]", distillery.Name.SanitizeForLog());
            return new CreateDistilleryResult(CreateDistilleryOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<UpdateDistilleryResult> UpdateAsync(
        Guid id,
        Distillery distillery,
        CancellationToken ct = default)
    {
        try
        {
            var current = await read.GetByIdAsync(id, ct);
            if (current is null)
            {
                logger.LogWarning("Distillery not found with [id: {Id}]", id);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.NotFound);
            }

            if (!string.Equals(current.Name, distillery.Name, StringComparison.Ordinal))
            {
                var exists = await read.ExistsByNameAsync(distillery.Name, ct);
                if (exists)
                {
                    logger.LogWarning(
                        "Distillery already exists with [Name: {Name}]",
                        distillery.Name.SanitizeForLog());
                    return new UpdateDistilleryResult(UpdateDistilleryOutcome.NameConflict);
                }
            }
            
            var updated = await write.UpdateAsync(id, distillery, ct);

            if (updated)
            {
                logger.LogDebug(
                    "Distillery updated with [Name: {Name}, Id: {Id}]",
                    distillery.Name.SanitizeForLog(),
                    id);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.Updated, distillery);
            }

            logger.LogWarning("Distillery not found with [id: {Id}]", id);
            return new UpdateDistilleryResult(UpdateDistilleryOutcome.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating distillery [Name: {Name}, Id: {Id}]",
                distillery.Name.SanitizeForLog(),
                id);
            return new UpdateDistilleryResult(UpdateDistilleryOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<DeleteDistilleryResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await write.DeleteAsync(id, ct);
            if (deleted)
            {
                logger.LogDebug("Distillery deleted with [Id: {Id}]", id);
                return new DeleteDistilleryResult(DeleteDistilleryOutcome.Deleted);
            }

            logger.LogWarning("Distillery not found with [Id: {Id}]", id);
            return new DeleteDistilleryResult(DeleteDistilleryOutcome.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting distillery [Id: {Id}]", id);
            return new DeleteDistilleryResult(DeleteDistilleryOutcome.Error, ex.Message);
        }
    }
}