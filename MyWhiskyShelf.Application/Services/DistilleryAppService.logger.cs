using Microsoft.Extensions.Logging;

namespace MyWhiskyShelf.Application.Services;

public partial class DistilleryAppService
{
    [LoggerMessage(LogLevel.Warning, "Distillery not found with [Id: {Id}]")]
    static partial void LogDistilleryNotFound(ILogger<DistilleryAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Debug, "Retrieved distillery with [Name: {Name}, Id: {Id}]")]
    static partial void LogRetrievedDistillery(ILogger<DistilleryAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Error, "Error retrieving distillery with [Id: {Id}]")]
    static partial void LogErrorRetrievingDistillery(ILogger<DistilleryAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Warning, "Invalid 'afterCursor' supplied. Cursor={Cursor}")]
    static partial void LogInvalidAfterCursorSupplied(ILogger<DistilleryAppService> logger, string cursor);

    [LoggerMessage(LogLevel.Error, "An error occurred whilst retrieving all distilleries")]
    static partial void LogErrorRetrievingAllDistilleries(ILogger<DistilleryAppService> logger);

    [LoggerMessage(LogLevel.Warning, "Distillery already exists with [Name: {Name}]")]
    static partial void LogDistilleryAlreadyExistsWithName(ILogger<DistilleryAppService> logger, string name);

    [LoggerMessage(LogLevel.Warning, "Country not found with [Id: {Id}]")]
    static partial void LogCountryNotFound(ILogger<DistilleryAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Debug, "Distillery created with [Name: {Name}, Id: {Id}]")]
    static partial void LogDistilleryCreated(ILogger<DistilleryAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Error, "Error creating distillery with [Name: {Name}]")]
    static partial void LogErrorCreatingDistillery(ILogger<DistilleryAppService> logger, string name);

    [LoggerMessage(LogLevel.Debug, "Distillery updated with [Name: {Name}, Id: {Id}]")]
    static partial void LogDistilleryUpdated(ILogger<DistilleryAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Error, "Error updating distillery [Name: {Name}, Id: {Id}]")]
    static partial void LogErrorUpdatingDistillery(ILogger<DistilleryAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Debug, "Distillery deleted with [Id: {Id}]")]
    static partial void LogDistilleryDeleted(ILogger<DistilleryAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Error, "Error deleting distillery [Id: {Id}]")]
    static partial void LogErrorDeletingDistillery(ILogger<DistilleryAppService> logger, Guid id);
    
    [LoggerMessage(LogLevel.Debug, "Retrieved [{Count}] distilleries (amount {Amount}, hasNext: {HasNext})")]
    static partial void LogRetrievedDistilleriesCount(
        ILogger<DistilleryAppService> logger,
        int count,
        int amount,
        bool hasNext);
    
    [LoggerMessage(LogLevel.Warning, "Region does not exist country [RegionId: {RegionId}, CountryId: {CountryId}]")]
    static partial void LogRegionDoesNotExistInCountry(
        ILogger<DistilleryAppService> logger,
        string regionId,
        string countryId);
}