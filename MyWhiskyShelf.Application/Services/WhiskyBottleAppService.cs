using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Application.Results.WhiskyBottles;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Services;

public sealed class WhiskyBottleAppService(
    IWhiskyBottleReadRepository read,
    IWhiskyBottleWriteRepository write,
    ILogger<WhiskyBottleAppService> logger) : IWhiskyBottleAppService
{
    public async Task<GetWhiskyBottleByIdResult> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var whiskyBottle = await read.GetByIdAsync(id, ct);

            if (whiskyBottle is null)
            {
                logger.LogWarning("Whisky bottle not found with [Id: {Id}]", id);
                return new GetWhiskyBottleByIdResult(GetWhiskyBottleByIdOutcome.NotFound);
            }

            logger.LogDebug(
                "Retrieved whisky bottle with [Name: {Name}, Id: {Id}]",
                whiskyBottle.Name.SanitizeForLog(),
                id);
            return new GetWhiskyBottleByIdResult(GetWhiskyBottleByIdOutcome.Success, whiskyBottle);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving distillery with [Id: {Id}]", id);
            return new GetWhiskyBottleByIdResult(GetWhiskyBottleByIdOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<CreateWhiskyBottleResult> CreateAsync(WhiskyBottle whiskyBottle, CancellationToken ct = default)
    {
        try
        {
            var addedWhiskyBottle = await write.AddAsync(whiskyBottle, ct);

            logger.LogDebug(
                "Whisky bottle created with [Name: {Name}, Id: {Id}]",
                addedWhiskyBottle.Name.SanitizeForLog(),
                addedWhiskyBottle.Id);
            return new CreateWhiskyBottleResult(CreateWhiskyBottleOutcome.Created, addedWhiskyBottle);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating whisky bottle with [Name: {Name}]", whiskyBottle.Name.SanitizeForLog());
            return new CreateWhiskyBottleResult(CreateWhiskyBottleOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<UpdateWhiskyBottleResult> UpdateAsync(
        Guid id,
        WhiskyBottle whiskyBottle,
        CancellationToken ct = default)
    {
        try
        {
            var updated = await write.UpdateAsync(id, whiskyBottle, ct);
            if (updated)
            {
                logger.LogDebug(
                    "Whisky bottle updated with [Name: {Name}, Id: {Id}]",
                    whiskyBottle.Name.SanitizeForLog(),
                    id);
                return new UpdateWhiskyBottleResult(UpdateWhiskyBottleOutcome.Updated, whiskyBottle with { Id = id });
            }

            logger.LogWarning("Whisky bottle not found with [id: {Id}]", id);
            return new UpdateWhiskyBottleResult(UpdateWhiskyBottleOutcome.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error updating whisky bottle [Name: {Name}, Id: {Id}]",
                whiskyBottle.Name.SanitizeForLog(),
                id);
            return new UpdateWhiskyBottleResult(UpdateWhiskyBottleOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<DeleteWhiskyBottleResult> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var deleted = await write.DeleteAsync(id, ct);
            if (deleted)
            {
                logger.LogDebug("Whisky bottle deleted with [Id: {Id}]", id);
                return new DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome.Deleted);
            }

            logger.LogWarning("Whisky bottle not found with [Id: {Id}]", id);
            return new DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting whisky bottle [Id: {Id}]", id);
            return new DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome.Error, ex.Message);
        }
    }
}