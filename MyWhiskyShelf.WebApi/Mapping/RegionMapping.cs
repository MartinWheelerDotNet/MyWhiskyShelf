using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;
using Slugify;

namespace MyWhiskyShelf.WebApi.Mapping;

public static class RegionMapping
{
    private static readonly SlugHelper SlugHelper = new();
    public static RegionResponse ToResponse(this Region region)
    {
        return new RegionResponse
        {
            Id = region.Id,
            CountryId = region.CountryId,
            Name = region.Name,
            Slug = region.Slug,
            IsActive = region.IsActive
        };
    }

    public static Region ToDomain(this RegionCreateRequest request)
    {
        return new Region
        {
            Id = request.Id,
            CountryId = request.CountryId,
            Name = request.Name,
            Slug = SlugHelper.GenerateSlug(request.Name),
            IsActive = request.IsActive
        };
    }
    
    public static Region ToDomain(this UpdateRegionRequest request)
    {
        return new Region
        {
            Id = request.Id,
            CountryId = request.CountryId,
            Name = request.Name,
            Slug = request.Slug,
            IsActive = request.IsActive
        };
    }
}