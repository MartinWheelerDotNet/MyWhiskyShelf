using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IGeoWriteRepository
{
    Task<Country> AddCountryAsync(Country country, CancellationToken ct = default);
    Task<bool> UpdateCountryAsync(Guid id, Country country, CancellationToken ct = default);
    Task<bool> SetCountryActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    Task<Region?> AddRegionAsync(Guid countryId, Region region, CancellationToken ct = default);
    Task<bool> UpdateRegionAsync(Guid id, Region region, CancellationToken ct = default);
    Task<bool> SetRegionActiveAsync(Guid id, bool isActive, CancellationToken ct = default);
}