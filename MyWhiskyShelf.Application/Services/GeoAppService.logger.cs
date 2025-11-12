using Microsoft.Extensions.Logging;

namespace MyWhiskyShelf.Application.Services;

public partial class GeoAppService
{
        [LoggerMessage(LogLevel.Error, "Error retrieving country and region information")]
    static partial void LogErrorRetrievingCountryAndRegionInformation(ILogger<GeoAppService> logger);

    [LoggerMessage(LogLevel.Information, "Country created with [Name: {Name}]")]
    static partial void LogCountryCreated(ILogger<GeoAppService> logger, string name);

    [LoggerMessage(LogLevel.Error, "Error creating country with [Name: {Name}]")]
    static partial void LogErrorCreatingCountry(ILogger<GeoAppService> logger, string name);

    [LoggerMessage(LogLevel.Error, "{Id}, {CountryId}")]
    static partial void LogIdCountryid(ILogger<GeoAppService> logger, Guid id, Guid countryId);

    [LoggerMessage(LogLevel.Information, "Region created with [CountryId: {CountryId}, Name: {Name}]")]
    static partial void LogRegionCreated(ILogger<GeoAppService> logger, Guid countryId, string name);

    [LoggerMessage(LogLevel.Error, "Error creating region [CountryId: {CountryId}, Name: {Name}]")]
    static partial void LogErrorCreatingRegion(ILogger<GeoAppService> logger, Guid countryId, string name);

    [LoggerMessage(LogLevel.Information, "Country updated with [Id: {Id}, Name: {name}]")]
    static partial void LogCountryUpdated(ILogger<GeoAppService> logger, Guid id, string name);

    [LoggerMessage(LogLevel.Error, "Error updating Country with [Id: {id}]")]
    static partial void LogErrorUpdatingCountry(ILogger<GeoAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Information, "Country active flag updated for [Id: {id}, IsActive: {isActive}]")]
    static partial void LogCountryActiveFlagUpdated(ILogger<GeoAppService> logger, Guid id, bool isActive);

    [LoggerMessage(LogLevel.Error, "Error setting country active flag for [Id: {id}]")]
    static partial void LogErrorSettingCountryActiveFlag(ILogger<GeoAppService> logger, Guid id);

    [LoggerMessage(LogLevel.Information, "Region updated with [CountryId: {id}, Name: {name}]")]
    static partial void LogRegionUpdated(ILogger<GeoAppService> logger, Guid id, string name);

    [LoggerMessage(LogLevel.Error, "Error updating Region with [Id: {id}, Name: {name}]")]
    static partial void LogErrorUpdatingRegion(ILogger<GeoAppService> logger, Guid id, string name);

    [LoggerMessage(LogLevel.Information, "Region active flag updated for [Id: {id}, IsActive: {isActive}]")]
    static partial void LogRegionActiveFlagUpdated(ILogger<GeoAppService> logger, Guid id, bool isActive);

    [LoggerMessage(LogLevel.Error, "Error setting region active flag for [Id: {id}]")]
    static partial void LogErrorSettingRegionActiveFlag(ILogger<GeoAppService> logger, Guid id);
}