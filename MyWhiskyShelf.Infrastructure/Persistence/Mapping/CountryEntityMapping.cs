using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Mapping;

public static class CountryEntityMapping
{
    public static Country ToDomain(this CountryEntity entity)
    {
        return new Country
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            IsActive = entity.IsActive,
            Regions = entity.Regions
                .OrderBy(r => r.Name)
                .ThenBy(r => r.Id)
                .Select(r => r.ToDomain())
                .ToList()
        };
    }

    public static CountryEntity ToEntity(this Country country)
    {
        return new CountryEntity
        {
            Name = country.Name.Trim(),
            Slug = country.Slug.Trim(),
            IsActive = country.IsActive
        };
    }
}