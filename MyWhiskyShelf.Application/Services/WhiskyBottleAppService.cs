using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Application.Results.WhiskyBottles;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Services;

public sealed partial class WhiskyBottleAppService(
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
                LogWhiskyBottleNotFound(logger, id);
                return new GetWhiskyBottleByIdResult(GetWhiskyBottleByIdOutcome.NotFound);
            }

            LogRetrievedWhiskyBottle(logger, whiskyBottle.Name.SanitizeForLog(), id);
            return new GetWhiskyBottleByIdResult(GetWhiskyBottleByIdOutcome.Success, whiskyBottle);
        }
        catch (Exception ex)
        {
            LogErrorRetrievingDistillery(logger, id);
            return new GetWhiskyBottleByIdResult(GetWhiskyBottleByIdOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<CreateWhiskyBottleResult> CreateAsync(WhiskyBottle whiskyBottle, CancellationToken ct = default)
    {
        try
        {
            var addedWhiskyBottle = await write.AddAsync(whiskyBottle, ct);

            LogWhiskyBottleCreated(logger, addedWhiskyBottle.Name.SanitizeForLog(), addedWhiskyBottle.Id);
            return new CreateWhiskyBottleResult(CreateWhiskyBottleOutcome.Created, addedWhiskyBottle);
        }
        catch (Exception ex)
        {
            LogErrorCreatingWhiskyBottle(logger, whiskyBottle.Name.SanitizeForLog());
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
                LogWhiskyBottleUpdated(logger, whiskyBottle.Name.SanitizeForLog(), id);
                return new UpdateWhiskyBottleResult(UpdateWhiskyBottleOutcome.Updated, whiskyBottle with { Id = id });
            }

            LogWhiskyBottleNotFound(logger, id);
            return new UpdateWhiskyBottleResult(UpdateWhiskyBottleOutcome.NotFound);
        }
        catch (Exception ex)
        {
            LogErrorUpdatingWhiskyBottle(logger, whiskyBottle.Name.SanitizeForLog(), id);
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
                LogWhiskyBottleDeleted(logger, id);
                return new DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome.Deleted);
            }

            LogWhiskyBottleNotFound(logger, id);
            return new DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome.NotFound);
        }
        catch (Exception ex)
        {
            LogErrorDeletingWhiskyBottle(logger, id);
            return new DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome.Error, ex.Message);
        }
    }
}