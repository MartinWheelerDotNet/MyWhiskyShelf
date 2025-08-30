using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Services;

public sealed class DistilleryAppService(
    IDistilleryReadRepository read,
    IDistilleryWriteRepository write,
    ILogger<DistilleryAppService> logger)
    : IDistilleryAppService
{
    public async Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var distillery = await read.GetByIdAsync(id, ct);

        if (distillery is null)
        {
            logger.LogWarning("Distillery not found with [Id: {Id}]", id);
            return null;
        }

        logger.LogDebug("Retrieved distillery with [Name: {Name}, Id: {Id}]", distillery.Name, id);
        return distillery;
    }


    public async Task<IReadOnlyList<Distillery>> GetAllAsync(CancellationToken ct = default)
    {
        var distilleries = await read.GetAllAsync(ct);

        logger.LogDebug("Retrieved [{Count}] distilleries", distilleries.Count);
        return distilleries;
    }


    public async Task<CreateDistilleryResult> CreateAsync(Distillery distillery, CancellationToken ct = default)
    {
        var exists = await read.ExistsByNameAsync(distillery.Name, ct);
        if (exists)
        {
            logger.LogWarning("Distillery already exists with [Name: {Name}]", distillery.Name);
            return new CreateDistilleryResult(CreateDistilleryOutcome.AlreadyExists);
        }

        try
        {
            var addedDistillery = await write.AddAsync(distillery, ct);

            logger.LogDebug(
                "Distillery created with [Name: {Name}, Id: {Id}]",
                addedDistillery!.Name,
                addedDistillery.Id);
            return new CreateDistilleryResult(CreateDistilleryOutcome.Created, addedDistillery);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating distillery with [Name: {Name}]", distillery.Name);
            return new CreateDistilleryResult(CreateDistilleryOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<UpdateDistilleryResult> UpdateAsync(
        Guid id,
        Distillery distillery,
        CancellationToken ct = default)
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
                logger.LogWarning("Distillery already exists with [Name: {Name}]", distillery.Name);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.NameConflict);
            }
        }

        try
        {
            var updated = await write.UpdateAsync(id, distillery, ct);

            if (updated)
            {
                logger.LogDebug("Distillery updated with [Name: {Name}, Id: {Id}]", distillery.Name, id);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.Updated, distillery);
            }

            logger.LogWarning("Distillery not found with [id: {Id}]", id);
            return new UpdateDistilleryResult(UpdateDistilleryOutcome.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error updating distillery [Name: {Name}, Id: {Id}]", distillery.Name, id);
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