using MyWhiskyShelf.Database.Models;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Extensions;

public static class EntityProjectionExtensions
{
    public static Distillery ProjectToDistillery(this DistilleryEntity distilleryEntity)
        => new()
        {
            DistilleryName = distilleryEntity.DistilleryName,
            Location = distilleryEntity.Location,
            Region = distilleryEntity.Region,
            Founded = distilleryEntity.Founded,
            Owner = distilleryEntity.Owner,
            DistilleryType = distilleryEntity.DistilleryType,
            Active = distilleryEntity.Active
        };

    public static DistilleryEntity ProjectToDistilleryEntity(this Distillery distillery)
        => new()
        {
            DistilleryName = distillery.DistilleryName,
            Location = distillery.Location,
            Region = distillery.Region,
            Founded = distillery.Founded,
            Owner = distillery.Owner,
            DistilleryType = distillery.DistilleryType,
            Active = distillery.Active
        };
}