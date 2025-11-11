using System.Linq.Expressions;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Projections;

public static class BrandProjections
{
    public static readonly Expression<Func<BrandEntity, Brand>> ToDomain = distilleryEntity => new Brand
    {
        Id = distilleryEntity.Id,
        Name = distilleryEntity.Name,
        Description = distilleryEntity.Description,
        CountryId = distilleryEntity.CountryId,
        CountryName = distilleryEntity.Country == null ? null: distilleryEntity.Country.Name
    };
}