using MyWhiskyShelf.Application.Results.Geo;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Services;

public interface IGeoAppService
{
    Task<GetCountryGeoResult> GetAllAsync(CancellationToken ct = default);

    Task<CreateCountryResult> CreateCountryAsync(Country country, CancellationToken ct = default);
    Task<UpdateCountryResult> UpdateCountryAsync(Guid id, Country updatedCountry, CancellationToken ct = default);
    Task<SetCountryActiveResult> SetCountryActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    Task<CreateRegionResult> CreateRegionAsync(Guid countryId, Region region, CancellationToken ct = default);
    Task<UpdateRegionResult> UpdateRegionAsync(Guid id, Region updatedRegion, CancellationToken ct = default);
    Task<SetRegionActiveResult> SetRegionActiveAsync(Guid id, bool isActive, CancellationToken ct = default);
}