using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Contracts.Brands;

namespace MyWhiskyShelf.WebApi.Mapping;

public static class BrandMapping
{
    public static BrandResponse ToResponse(this Brand brand)
    {
        return new BrandResponse
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description,
            CountryId = brand.CountryId,
            CountryName = brand.CountryName
        };
    }
}