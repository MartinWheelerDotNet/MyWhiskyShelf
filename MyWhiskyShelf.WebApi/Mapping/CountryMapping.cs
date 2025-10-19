using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;
using Slugify;

namespace MyWhiskyShelf.WebApi.Mapping;

public static class CountryMapping
{
    private static readonly SlugHelper SlugHelper = new();

    public static CountryResponse ToResponse(this Country domain)
    {
        return new CountryResponse
        {
            Id = domain.Id,
            Name = domain.Name,
            Slug = domain.Slug,
            IsActive = domain.IsActive,
            Regions = domain.Regions.Select(r => r.ToResponse()).ToList()
        };
    }

    public static Country ToDomain(this UpdateCountryRequest request)
    {
        return new Country
        {
            Id = request.Id,
            Name = request.Name,
            Slug = request.Slug,
            IsActive = request.IsActive
        };
    }

    public static Country ToDomain(this CountryCreateRequest request)
    {
        return new Country
        {
            Name = request.Name,
            Slug = SlugHelper.GenerateSlug(request.Name),
            IsActive = request.IsActive
        };
    }
}