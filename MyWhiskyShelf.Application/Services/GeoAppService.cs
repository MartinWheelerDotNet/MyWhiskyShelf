using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Application.Results.GeoData;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Services;

public sealed partial class GeoAppService(
    IGeoReadRepository read,
    IGeoWriteRepository write,
    ILogger<GeoAppService> logger) : IGeoAppService
{
    public async Task<GetCountryGeoResult> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            var countries = await read.GetAllGeoInformationAsync(ct);
            return new GetCountryGeoResult(GetCountryGeoOutcome.Success, countries);
        }
        catch (Exception ex)
        {
            LogErrorRetrievingCountryAndRegionInformation(logger);
            return new GetCountryGeoResult(GetCountryGeoOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<CreateCountryResult> CreateCountryAsync(Country country, CancellationToken ct = default)
    {
        try
        {
            if (await DoesNameExist(country, ct))
                return new CreateCountryResult(CreateCountryOutcome.NameConflict);

            var createdCountry = await write.AddCountryAsync(country, ct);

            LogCountryCreated(logger, createdCountry.Name.SanitizeForLog());
            return new CreateCountryResult(CreateCountryOutcome.Created, createdCountry);
        }
        catch (Exception ex)
        {
            LogErrorCreatingCountry(logger, country.Name.SanitizeForLog());
            return new CreateCountryResult(CreateCountryOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<CreateRegionResult> CreateRegionAsync(Guid countryId, Region region,
        CancellationToken ct = default)
    {
        try
        {
            if (!await read.CountryExistsByIdAsync(countryId, ct))
                return new CreateRegionResult(CreateRegionOutcome.CountryNotFound);

            if (await DoesNameExist(region, ct))
                return new CreateRegionResult(CreateRegionOutcome.NameConflict);
            
            var createdRegion = await write.AddRegionAsync(countryId, region, ct);
            if (createdRegion is null)
                return new CreateRegionResult(CreateRegionOutcome.CountryNotFound);

            LogRegionCreated(logger, countryId, createdRegion.Name.SanitizeForLog());
            return new CreateRegionResult(CreateRegionOutcome.Created, createdRegion);
        }
        catch (Exception ex)
        {
            LogErrorCreatingRegion(logger, countryId, region.Name.SanitizeForLog());
            return new CreateRegionResult(CreateRegionOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<UpdateCountryResult> UpdateCountryAsync(Guid id, Country updatedCountry,
        CancellationToken ct = default)
    {
        try
        {
            var currentCountry = await read.GetCountryByIdAsync(id, ct);

            if (currentCountry is null)
                return new UpdateCountryResult(UpdateCountryOutcome.NotFound);

            if (HasNameChanged(currentCountry, updatedCountry) && await DoesNameExist(updatedCountry, ct))
                return new UpdateCountryResult(UpdateCountryOutcome.NameConflict);

            if (!await write.UpdateCountryAsync(id, updatedCountry, ct))
                return new UpdateCountryResult(UpdateCountryOutcome.NotFound);

            LogCountryUpdated(logger, id, updatedCountry.Name.SanitizeForLog());
            return new UpdateCountryResult(UpdateCountryOutcome.Updated, updatedCountry);
        }
        catch (Exception ex)
        {
            LogErrorUpdatingCountry(logger, id);
            return new UpdateCountryResult(UpdateCountryOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<SetCountryActiveResult> SetCountryActiveAsync(Guid id, bool isActive,
        CancellationToken ct = default)
    {
        try
        {
            if (!await write.SetCountryActiveAsync(id, isActive, ct))
                return new SetCountryActiveResult(SetCountryActiveOutcome.NotFound);

            LogCountryActiveFlagUpdated(logger, id, isActive);
            return new SetCountryActiveResult(SetCountryActiveOutcome.Updated);
        }
        catch (Exception ex)
        {
            LogErrorSettingCountryActiveFlag(logger, id);
            return new SetCountryActiveResult(SetCountryActiveOutcome.Error, ex.Message);
        }
    }

    public async Task<UpdateRegionResult> UpdateRegionAsync(Guid id, Region updatedRegion,
        CancellationToken ct = default)
    {
        try
        {
            var currentRegion = await read.GetRegionByIdAsync(id, ct);

            if (currentRegion is null)
                return new UpdateRegionResult(UpdateRegionOutcome.NotFound);

            if (HasCountryChanged(currentRegion, updatedRegion))
                return new UpdateRegionResult(UpdateRegionOutcome.CountryChangeAttempted);

            if (HasNameChanged(currentRegion, updatedRegion) && await DoesNameExist(updatedRegion, ct))
                return new UpdateRegionResult(UpdateRegionOutcome.NameConflict);
            
            if (!await write.UpdateRegionAsync(id, updatedRegion, ct))
                return new UpdateRegionResult(UpdateRegionOutcome.NotFound);

            LogRegionUpdated(logger, updatedRegion.CountryId, updatedRegion.Name.SanitizeForLog());

            return new UpdateRegionResult(UpdateRegionOutcome.Updated, updatedRegion);
        }
        catch (Exception ex)
        {
            LogErrorUpdatingRegion(logger, id, updatedRegion.Name.SanitizeForLog());
            return new UpdateRegionResult(UpdateRegionOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<SetRegionActiveResult> SetRegionActiveAsync(Guid id, bool isActive,
        CancellationToken ct = default)
    {
        try
        {
            if (!await write.SetRegionActiveAsync(id, isActive, ct))
                return new SetRegionActiveResult(SetRegionActiveOutcome.NotFound);

            LogRegionActiveFlagUpdated(logger, id, isActive);
            return new SetRegionActiveResult(SetRegionActiveOutcome.Updated);
        }
        catch (Exception ex)
        {
            LogErrorSettingRegionActiveFlag(logger, id);
            return new SetRegionActiveResult(SetRegionActiveOutcome.Error, ex.Message);
        }
    }

    private static bool HasNameChanged(Country currentCountry, Country updatedCountry)
    {
        return currentCountry.Name != updatedCountry.Name;
    }

    private static bool HasNameChanged(Region currentRegion, Region updatedRegion)
    {
        return currentRegion.Name != updatedRegion.Name;
    }
    
    private static bool HasCountryChanged(Region currentRegion, Region updatedRegion)
    {
        return currentRegion.CountryId != updatedRegion.CountryId;
    }

    private async Task<bool> DoesNameExist(Country country, CancellationToken ct = default)
    {
        return await read.CountryExistsByNameAsync(country.Name, ct);
    }

    private async Task<bool> DoesNameExist(Region region, CancellationToken ct = default)
    {
        return await read.RegionExistsByNameAndCountryIdAsync(region.Name, region.CountryId, ct);
    }
}