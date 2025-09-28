using System.Linq.Expressions;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Projections;

public static class DistilleryProjections
{
    public static readonly Expression<Func<DistilleryEntity, Distillery>> ToDistilleryDomain =
        distilleryEntity => new Distillery
        {
            Id = distilleryEntity.Id,
            Name = distilleryEntity.Name,
            Location = distilleryEntity.Location,
            Region = distilleryEntity.Region,
            Founded = distilleryEntity.Founded,
            Owner = distilleryEntity.Owner,
            Type = distilleryEntity.Type,
            FlavourProfile = distilleryEntity.FlavourProfile,
            Active = distilleryEntity.Active
        };

    public static readonly Expression<Func<DistilleryEntity, DistilleryName>> ToDistilleryNameDomain =
        distilleryEntity => new DistilleryName(distilleryEntity.Id, distilleryEntity.Name);
}