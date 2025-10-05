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
            Country = distilleryEntity.Country,
            Region = distilleryEntity.Region,
            Founded = distilleryEntity.Founded,
            Owner = distilleryEntity.Owner,
            Type = distilleryEntity.Type,
            Description = distilleryEntity.Description,
            TastingNotes = distilleryEntity.TastingNotes,
            FlavourProfile = distilleryEntity.FlavourVector.ToFlavourProfile(),
            Active = distilleryEntity.Active
        };
    }

    public static DistilleryEntity ToEntity(this Distillery distillery)
    {
        return new DistilleryEntity
        {
            Name = distillery.Name.Trim(),
            Country = distillery.Country,
            Region = distillery.Region,
            Founded = distillery.Founded,
            Owner = distillery.Owner,
            Type = distillery.Type,
            Description = distillery.Description,
            TastingNotes = distillery.TastingNotes,
            FlavourVector = distillery.FlavourProfile.ToVector(),
            Active = distillery.Active
        };
    }
}