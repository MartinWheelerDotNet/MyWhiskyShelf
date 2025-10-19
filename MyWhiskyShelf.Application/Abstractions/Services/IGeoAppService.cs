using MyWhiskyShelf.Application.Results.Geo;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Services;

public interface IGeoAppService
{
    Task<GetCountryGeoResult> GetAllAsync(CancellationToken ct = default);

    Task<CreateCountryResult> CreateCountryAsync(Country country, CancellationToken ct = default);

    Task<CreateRegionResult> CreateRegionAsync(Guid countryId, Region region, CancellationToken ct = default);
}