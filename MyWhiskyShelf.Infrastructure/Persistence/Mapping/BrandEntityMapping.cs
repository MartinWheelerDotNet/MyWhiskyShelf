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
            Description = entity.Description
        };
    }

    public static BrandEntity ToEntity(this Brand country)
    {
        return new BrandEntity
        {
            Id = country.Id,
            Name = country.Name.Trim(),
            Description =  country.Description?.Trim()
        };
    }
}