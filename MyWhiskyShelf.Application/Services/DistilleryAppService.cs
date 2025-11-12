using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Cursor;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Cursors;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Application.Results.Distilleries;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Services;

public sealed partial class DistilleryAppService(
    IDistilleryReadRepository read,
    IDistilleryWriteRepository write,
    IGeoReadRepository geoRead,
    ICursorCodec cursorCodec,
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
                LogDistilleryNotFound(logger, id);
                return new GetDistilleryByIdResult(GetDistilleryByIdOutcome.NotFound);
            }

            LogRetrievedDistillery(logger, distillery.Name.SanitizeForLog(), id);
            return new GetDistilleryByIdResult(GetDistilleryByIdOutcome.Success, distillery);
        }
        catch (Exception ex)
        {
            LogErrorRetrievingDistillery(logger, id);
            return new GetDistilleryByIdResult(GetDistilleryByIdOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<GetAllDistilleriesResult> GetAllAsync(
        int amount,
        string afterCursor,
        CancellationToken ct = default)
    {
        DistilleryQueryCursor? cursor = null;
        if (!string.IsNullOrWhiteSpace(afterCursor) && !cursorCodec.TryDecode(afterCursor, out cursor))
        {
            LogInvalidAfterCursorSupplied(logger, afterCursor.SanitizeForLog());
            return new GetAllDistilleriesResult(
                GetAllDistilleriesOutcome.InvalidCursor,
                Error: "Invalid cursor provided");
        }

        var queryOptions = new DistilleryFilterOptions(
            cursor?.CountryId,
            cursor?.RegionId,
            cursor?.NameSearchPattern,
            amount,
            cursor?.AfterName);

        return await GetAllAsync(queryOptions, ct);
    }

    public async Task<GetAllDistilleriesResult> GetAllAsync(
        DistilleryFilterOptions filterOptions,
        CancellationToken ct = default)
    {
        var amount = filterOptions.Amount;
        if (amount <= 0)
            return new GetAllDistilleriesResult(GetAllDistilleriesOutcome.Success, []);

        try
        {
            var items = await read.SearchByFilter(filterOptions, ct);
            var nextCursor = GenerateNextCursor(filterOptions, items, amount);

            LogRetrievedDistilleriesCount(logger, items.Count, amount, nextCursor is not null);

            return new GetAllDistilleriesResult(GetAllDistilleriesOutcome.Success, items, nextCursor, amount);
        }
        catch (Exception ex)
        {
            LogErrorRetrievingAllDistilleries(logger);
            return new GetAllDistilleriesResult(GetAllDistilleriesOutcome.Error, Error: ex.Message);
        }
    }
    
    public async Task<CreateDistilleryResult> CreateAsync(Distillery distillery, CancellationToken ct = default)
    {
        try
        {
            if (await read.ExistsByNameAsync(distillery.Name, ct))
            {
                LogDistilleryAlreadyExistsWithName(logger, distillery.Name.SanitizeForLog());
                return new CreateDistilleryResult(CreateDistilleryOutcome.AlreadyExists);
            }

            if (!await geoRead.CountryExistsByIdAsync(distillery.CountryId, ct))
            {
                LogCountryNotFound(logger, distillery.CountryId);
                return new CreateDistilleryResult(CreateDistilleryOutcome.CountryDoesNotExist);
            }

            if (distillery.RegionId is {} regionGuid 
                && !await RegionExistsAndIsInCountry(distillery.CountryId, regionGuid, ct))
            {
                LogRegionDoesNotExistInCountry(logger, distillery.RegionId?.SanitizeForLog() ?? string.Empty, distillery.CountryId.SanitizeForLog());
                return new CreateDistilleryResult(CreateDistilleryOutcome.RegionDoesNotExistInCountry);
            }
            
            var addedDistillery = await write.AddAsync(distillery, ct);

            LogDistilleryCreated(logger, addedDistillery.Name.SanitizeForLog(), addedDistillery.Id);
            return new CreateDistilleryResult(CreateDistilleryOutcome.Created, addedDistillery);
        }
        catch (Exception ex)
        {
            LogErrorCreatingDistillery(logger, distillery.Name.SanitizeForLog());
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
                LogDistilleryNotFound(logger, id);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.NotFound);
            }
            
            if (!string.Equals(current.Name, distillery.Name, StringComparison.Ordinal))
            {
                var exists = await read.ExistsByNameAsync(distillery.Name, ct);
                if (exists)
                {
                    LogDistilleryAlreadyExistsWithName(logger, distillery.Name.SanitizeForLog());
                    return new UpdateDistilleryResult(UpdateDistilleryOutcome.NameConflict);
                }
            }

            if (!await geoRead.CountryExistsByIdAsync(distillery.CountryId, ct))
            {
                LogCountryNotFound(logger, distillery.CountryId);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.CountryDoesNotExist);
            }

            if (distillery.RegionId is {} regionGuid 
                && !await RegionExistsAndIsInCountry(distillery.CountryId, regionGuid, ct))
            {
                LogRegionDoesNotExistInCountry(logger, distillery.RegionId?.SanitizeForLog() ?? string.Empty, distillery.CountryId.SanitizeForLog());
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.RegionDoesNotExistInCountry);
            }

            var updated = await write.UpdateAsync(id, distillery, ct);

            if (updated)
            {
                LogDistilleryUpdated(logger, distillery.Name.SanitizeForLog(), id);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.Updated, distillery);
            }

            LogDistilleryNotFound(logger, id);
            return new UpdateDistilleryResult(UpdateDistilleryOutcome.NotFound);
        }
        catch (Exception ex)
        {
            LogErrorUpdatingDistillery(logger, distillery.Name.SanitizeForLog(), id);
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
                LogDistilleryDeleted(logger, id);
                return new DeleteDistilleryResult(DeleteDistilleryOutcome.Deleted);
            }

            LogDistilleryNotFound(logger, id);
            return new DeleteDistilleryResult(DeleteDistilleryOutcome.NotFound);
        }
        catch (Exception ex)
        {
            LogErrorDeletingDistillery(logger, id);
            return new DeleteDistilleryResult(DeleteDistilleryOutcome.Error, ex.Message);
        }
    }

    private async Task<bool> RegionExistsAndIsInCountry(Guid countryId, Guid regionId, CancellationToken ct)
    {
        var region = await geoRead.GetRegionByIdAsync(regionId, ct);
        if (region is null) return false;

        return region.CountryId == countryId;
    }
    
    private string? GenerateNextCursor(DistilleryFilterOptions filterOptions, IReadOnlyList<Distillery> items, int amount)
        => items.Count < amount
            ? null
            : cursorCodec.Encode(new DistilleryQueryCursor(
                items[^1].Name,
                filterOptions.NameSearchPattern,
                filterOptions.CountryId,
                filterOptions.RegionId));
}