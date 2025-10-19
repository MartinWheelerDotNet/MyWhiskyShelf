using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Extensions;
using MyWhiskyShelf.Application.Results.Geo;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Services;

public sealed class GeoAppService(
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
            logger.LogError(ex, "Error retrieving country and region information");
            return new GetCountryGeoResult(GetCountryGeoOutcome.Error, Error: ex.Message);
        }
    }

    public async Task<CreateCountryResult> CreateCountryAsync(Country country, CancellationToken ct = default)
    {
        try
        {
            if (await DoesNameExist(country, ct))
                return new CreateCountryResult(CreateCountryOutcome.NameConflict);

            if (await DoesSlugExist(country, ct))
                country = country with { Slug = EnrichSlug(country.Slug) };

            var createdCountry = await write.AddCountryAsync(country, ct);

            logger.LogInformation(
                "Country created with [Name: {Name}, Slug: {Slug}]",
                createdCountry.Name.SanitizeForLog(),
                createdCountry.Slug.SanitizeForLog());
            return new CreateCountryResult(CreateCountryOutcome.Created, createdCountry);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error creating country with [Name: {Name}]",
                country.Name.SanitizeForLog());
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

            if (await DoesSlugExist(region, ct))
                region = region with { Slug = EnrichSlug(region.Slug) };

            var createdRegion = await write.AddRegionAsync(countryId, region, ct);
            if (createdRegion is null)
                return new CreateRegionResult(CreateRegionOutcome.CountryNotFound);

            logger.LogInformation(
                "Region created with [CountryId: {CountryId}, Name: {Name}]",
                countryId,
                createdRegion.Name.SanitizeForLog());
            return new CreateRegionResult(CreateRegionOutcome.Created, createdRegion);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error creating region [CountryId: {CountryId}, Name: {Name}]",
                countryId,
                region.Name.SanitizeForLog());
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

            if (HasSlugChanged(currentCountry, updatedCountry) && await DoesSlugExist(updatedCountry, ct))
                updatedCountry = updatedCountry with { Slug = EnrichSlug(updatedCountry.Slug) };

            if (!await write.UpdateCountryAsync(id, updatedCountry, ct))
                return new UpdateCountryResult(UpdateCountryOutcome.NotFound);

            logger.LogInformation(
                "Country updated with [Id: {Id}, Name: {Name}]",
                id,
                updatedCountry.Name.SanitizeForLog());
            return new UpdateCountryResult(UpdateCountryOutcome.Updated, updatedCountry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating Country with [Id: {Id}]", id);
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

            logger.LogInformation("Country active flag updated for [Id: {Id}, IsActive: {IsActive}]", id, isActive);
            return new SetCountryActiveResult(SetCountryActiveOutcome.Updated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting country active flag for [Id: {Id}]", id);
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

            if (HasSlugChanged(currentRegion, updatedRegion) && await DoesSlugExist(updatedRegion, ct))
                updatedRegion = updatedRegion with { Slug = EnrichSlug(updatedRegion.Slug) };

            if (!await write.UpdateRegionAsync(id, updatedRegion, ct))
                return new UpdateRegionResult(UpdateRegionOutcome.NotFound);

            logger.LogInformation(
                "Region updated with [CountryId: {Id}, Name: {Name}]",
                updatedRegion.CountryId,
                updatedRegion.Name.SanitizeForLog());

            return new UpdateRegionResult(UpdateRegionOutcome.Updated, updatedRegion);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error updating Region with [Id: {Id}, Name: {Name}]",
                id,
                updatedRegion.Name.SanitizeForLog());
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

            logger.LogInformation("Region active flag updated for [Id: {Id}, IsActive: {IsActive}]", id, isActive);
            return new SetRegionActiveResult(SetRegionActiveOutcome.Updated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting region active flag for [Id: {Id}]", id);
            return new SetRegionActiveResult(SetRegionActiveOutcome.Error, ex.Message);
        }
    }

    private static string EnrichSlug(string slug)
    {
        return $"{slug}-{DateTime.Now:yyyyMMddHHmmss}";
    }

    private static bool HasNameChanged(Country currentCountry, Country updatedCountry)
    {
        return currentCountry.Name != updatedCountry.Name;
    }

    private static bool HasNameChanged(Region currentRegion, Region updatedRegion)
    {
        return currentRegion.Name != updatedRegion.Name;
    }

    private static bool HasSlugChanged(Country currentCountry, Country updatedCountry)
    {
        return currentCountry.Slug != updatedCountry.Slug;
    }

    private static bool HasSlugChanged(Region currentRegion, Region updatedRegion)
    {
        return currentRegion.Slug != updatedRegion.Slug;
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

    private async Task<bool> DoesSlugExist(Country country, CancellationToken ct = default)
    {
        return await read.CountryExistsBySlugAsync(country.Slug, ct);
    }

    private async Task<bool> DoesSlugExist(Region region, CancellationToken ct = default)
    {
        return await read.RegionExistsBySlugAndCountryIdAsync(region.Slug, region.CountryId, ct);
    }
}