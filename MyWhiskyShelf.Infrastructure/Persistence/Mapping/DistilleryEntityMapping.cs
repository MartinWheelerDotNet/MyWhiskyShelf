using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Mapping;

public static class DistilleryEntityMapping
{
    public static Distillery ToDomain(this DistilleryEntity distilleryEntity)
    {
        return new Distillery
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
    }

    public static DistilleryEntity ToEntity(this Distillery distillery)
    {
        return new DistilleryEntity
        {
            Name = distillery.Name.Trim(),
            Location = distillery.Location,
            Region = distillery.Region,
            Founded = distillery.Founded,
            Owner = distillery.Owner,
            Type = distillery.Type,
            FlavourProfile = distillery.FlavourProfile,
            Active = distillery.Active
        };
    }
}