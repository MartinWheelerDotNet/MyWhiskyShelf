using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Cursor;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Cursors;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Application.Results.Distillery;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Services;

public sealed class DistilleryAppService(
    IDistilleryReadRepository read,
    IDistilleryWriteRepository write,
    IGeoReadRepository geoRead,
    ICursorCodec cursorCodec,
    ILogger<DistilleryAppService> logger)
    : IDistilleryAppService
{
    private const string RetrievedDistillery = 
        "Retrieved distillery with [Name: {Name}, Id: {Id}]";
    private const string RetrievedDistilleries = 
        "Retrieved [{Count}] distilleries (amount {Amount}, hasNext: {HasNext})";
    private const string DistilleryNotFound = 
        "Distillery not found with [Id: {Id}]";
    private const string ErrorRetrievingDistillery =
        "Error retrieving distillery with [Id: {Id}]";
    private const string InvalidAfterCursor =
        "Invalid 'afterCursor' supplied. Cursor={Cursor}";
    private const string ErrorRetrievingAllDistilleries =
        "An error occurred whilst retrieving all distilleries";
    private const string DistilleryAlreadyExistsWithName =
        "Distillery already exists with [Name: {Name}]";
    private const string CountryDoesNotExist =
        "Country not found with [Id: {Id}]";
    private const string DistilleryCreated =
        "Distillery created with [Name: {Name}, Id: {Id}]";
    private const string ErrorCreatingDistillery =
        "Error creating distillery with [Name: {Name}]";
    private const string DistilleryUpdated =
        "Distillery updated with [Name: {Name}, Id: {Id}]";
    private const string ErrorUpdatingDistillery =
        "Error updating distillery [Name: {Name}, Id: {Id}]";
    private const string DistilleryDeleted =
        "Distillery deleted with [Id: {Id}]";
    private const string ErrorDeletingDistillery = 
        "Error deleting distillery [Id: {Id}]";
    private const string RegionDoesNotExistInCountry = 
        "Region does not exist in the specified country [RegionId: {RegionId}, CountryId: {CountryId}]";

    public async Task<GetDistilleryByIdResult> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var distillery = await read.GetByIdAsync(id, ct);

            if (distillery is null)
            {
                logger.LogWarning(DistilleryNotFound, id);
                return new GetDistilleryByIdResult(GetDistilleryByIdOutcome.NotFound);
            }

            logger.LogDebug(RetrievedDistillery, distillery.Name.SanitizeForLog(), id);
            return new GetDistilleryByIdResult(GetDistilleryByIdOutcome.Success, distillery);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorRetrievingDistillery, id);
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
            logger.LogWarning(InvalidAfterCursor, afterCursor.SanitizeForLog());
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

            logger.LogDebug(
                RetrievedDistilleries,
                items.Count,
                amount,
                nextCursor is not null);

            return new GetAllDistilleriesResult(GetAllDistilleriesOutcome.Success, items, nextCursor, amount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorRetrievingAllDistilleries);
            return new GetAllDistilleriesResult(GetAllDistilleriesOutcome.Error, Error: ex.Message);
        }
    }
    
    public async Task<CreateDistilleryResult> CreateAsync(Distillery distillery, CancellationToken ct = default)
    {
        try
        {
            if (await read.ExistsByNameAsync(distillery.Name, ct))
            {
                logger.LogWarning(DistilleryAlreadyExistsWithName, distillery.Name.SanitizeForLog());
                return new CreateDistilleryResult(CreateDistilleryOutcome.AlreadyExists);
            }

            if (!await geoRead.CountryExistsByIdAsync(distillery.CountryId, ct))
            {
                logger.LogWarning(CountryDoesNotExist, distillery.CountryId);
                return new CreateDistilleryResult(CreateDistilleryOutcome.CountryDoesNotExist);
            }

            if (distillery.RegionId is {} regionGuid 
                && !await RegionExistsAndIsInCountry(distillery.CountryId, regionGuid, ct))
            {
                logger.LogWarning(RegionDoesNotExistInCountry, distillery.RegionId, distillery.CountryId);
                return new CreateDistilleryResult(CreateDistilleryOutcome.RegionDoesNotExistInCountry);
            }
            
            var addedDistillery = await write.AddAsync(distillery, ct);

            logger.LogDebug(DistilleryCreated, addedDistillery.Name.SanitizeForLog(), addedDistillery.Id);
            return new CreateDistilleryResult(CreateDistilleryOutcome.Created, addedDistillery);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorCreatingDistillery, distillery.Name.SanitizeForLog());
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
                logger.LogWarning(DistilleryNotFound, id);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.NotFound);
            }
            
            if (!string.Equals(current.Name, distillery.Name, StringComparison.Ordinal))
            {
                var exists = await read.ExistsByNameAsync(distillery.Name, ct);
                if (exists)
                {
                    logger.LogWarning(DistilleryAlreadyExistsWithName, distillery.Name.SanitizeForLog());
                    return new UpdateDistilleryResult(UpdateDistilleryOutcome.NameConflict);
                }
            }

            if (!await geoRead.CountryExistsByIdAsync(distillery.CountryId, ct))
            {
                logger.LogWarning(CountryDoesNotExist, distillery.CountryId);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.CountryDoesNotExist);
            }

            if (distillery.RegionId is {} regionGuid 
                && !await RegionExistsAndIsInCountry(distillery.CountryId, regionGuid, ct))
            {
                logger.LogWarning(RegionDoesNotExistInCountry, distillery.RegionId, distillery.CountryId);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.RegionDoesNotExistInCountry);
            }

            var updated = await write.UpdateAsync(id, distillery, ct);

            if (updated)
            {
                logger.LogDebug(DistilleryUpdated, distillery.Name.SanitizeForLog(), id);
                return new UpdateDistilleryResult(UpdateDistilleryOutcome.Updated, distillery);
            }

            logger.LogWarning(DistilleryNotFound, id);
            return new UpdateDistilleryResult(UpdateDistilleryOutcome.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorUpdatingDistillery, distillery.Name.SanitizeForLog(), id);
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
                logger.LogDebug(DistilleryDeleted, id);
                return new DeleteDistilleryResult(DeleteDistilleryOutcome.Deleted);
            }

            logger.LogWarning(DistilleryNotFound, id);
            return new DeleteDistilleryResult(DeleteDistilleryOutcome.NotFound);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorDeletingDistillery, id);
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