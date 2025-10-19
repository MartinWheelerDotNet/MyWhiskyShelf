using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IGeoReadRepository
{
    Task<IReadOnlyList<Country>> GetAllGeoInformationAsync(CancellationToken ct = default);

    Task<Country?> GetCountryByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> CountryExistsByNameAsync(string name, CancellationToken ct = default);
    Task<bool> CountryExistsBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> CountryExistsByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<Region?> GetRegionByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> RegionExistsByNameAndCountryIdAsync(string name, Guid countryId, CancellationToken ct = default);
    Task<bool> RegionExistsBySlugAndCountryIdAsync(string slug, Guid countryId, CancellationToken ct = default);
    Task<bool> RegionExistsByIdAsync(Guid id, CancellationToken ct = default);
}