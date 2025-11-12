using Microsoft.Extensions.Logging;

namespace MyWhiskyShelf.Application.Services;

public partial class WhiskyBottleAppService
{
    [LoggerMessage(LogLevel.Warning, "Whisky bottle not found with [Id: {Id}]")]
    static partial void LogWhiskyBottleNotFound(ILogger<WhiskyBottleAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Debug, "Retrieved whisky bottle with [Name: {Name}, Id: {Id}]")]
    static partial void LogRetrievedWhiskyBottle(ILogger<WhiskyBottleAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Error, "Error retrieving distillery with [Id: {Id}]")]
    static partial void LogErrorRetrievingDistillery(ILogger<WhiskyBottleAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Debug, "Whisky bottle created with [Name: {Name}, Id: {Id}]")]
    static partial void LogWhiskyBottleCreated(ILogger<WhiskyBottleAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Error, "Error creating whisky bottle with [Name: {Name}]")]
    static partial void LogErrorCreatingWhiskyBottle(ILogger<WhiskyBottleAppService> logger, string name);

    [LoggerMessage(LogLevel.Debug, "Whisky bottle updated with [Name: {Name}, Id: {Id}]")]
    static partial void LogWhiskyBottleUpdated(ILogger<WhiskyBottleAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Error, "Error updating whisky bottle [Name: {Name}, Id: {Id}]")]
    static partial void LogErrorUpdatingWhiskyBottle(ILogger<WhiskyBottleAppService> logger, string name, Guid id);

    [LoggerMessage(LogLevel.Debug, "Whisky bottle deleted with [Id: {Id}]")]
    static partial void LogWhiskyBottleDeleted(ILogger<WhiskyBottleAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Error, "Error deleting whisky bottle [Id: {Id}]")]
    static partial void LogErrorDeletingWhiskyBottle(ILogger<WhiskyBottleAppService> logger, Guid id);
}