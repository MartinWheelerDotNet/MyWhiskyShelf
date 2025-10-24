using System.Linq.Expressions;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.Infrastructure.Persistence.Projections;

public static class DistilleryProjections
{
    public static readonly Expression<Func<DistilleryEntity, Distillery>> ToDistilleryDomain =
        distilleryEntity => new Distillery
        {
            Id = distilleryEntity.Id,
            Name = distilleryEntity.Name,
            CountryId = distilleryEntity.CountryId,
            CountryName = distilleryEntity.Country.Name,
            RegionId = distilleryEntity.RegionId,
            RegionName = distilleryEntity.Region == null ? null : distilleryEntity.Region.Name,
            Founded = distilleryEntity.Founded,
            Owner = distilleryEntity.Owner,
            Type = distilleryEntity.Type,
            Description = distilleryEntity.Description,
            TastingNotes = distilleryEntity.TastingNotes,
            FlavourProfile = distilleryEntity.FlavourVector.ToFlavourProfile(),
            Active = distilleryEntity.Active
        };
}