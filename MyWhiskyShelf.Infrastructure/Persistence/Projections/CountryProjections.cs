using System.Linq.Expressions;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Projections;

public static class CountryProjections
{
    public static readonly Expression<Func<CountryEntity, Country>> ToCountryDomain =
        entity => new Country
        {
            Id = entity.Id,
            Name = entity.Name,
            IsActive = entity.IsActive,
            Regions = entity.Regions
                .OrderBy(r => r.Name)
                .ThenBy(r => r.Id)
                .Select(r => new Region
                {
                    Id = r.Id,
                    CountryId = r.CountryId,
                    Name = r.Name,
                    IsActive = r.IsActive
                })
                .ToList()
        };
}