using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Mapping;

public static class BrandEntityMapping
{
    public static Brand ToDomain(this BrandEntity entity)
    {
        return new Brand
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CountryId = entity.CountryId,
            CountryName = entity.Country?.Name
        };
    }

    public static BrandEntity ToEntity(this Brand brand)
    {
        return new BrandEntity
        {
            Id = brand.Id,
            Name = brand.Name.Trim(),
            Description =  brand.Description?.Trim(),
            CountryId =  brand.CountryId
        };
    }
}