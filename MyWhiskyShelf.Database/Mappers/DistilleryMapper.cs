using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class DistilleryMapper : IDistilleryMapper
{
    public DistilleryEntity MapToEntity(Distillery distillery) => new()
    {
        DistilleryName = distillery.DistilleryName,
        Location = distillery.Location,
        Region = distillery.Region,
        Founded = distillery.Founded,
        Owner = distillery.Owner,
        DistilleryType = distillery.DistilleryType,
        EncodedFlavourProfile = FlavourProfileEncoder.Encode(distillery.FlavourProfile),
        Active = distillery.Active
    };

    public Distillery MapToDomain(DistilleryEntity distilleryEntity) => new()
    {
        DistilleryName = distilleryEntity.DistilleryName,
        Location = distilleryEntity.Location,
        Region = distilleryEntity.Region,
        Founded = distilleryEntity.Founded,
        Owner = distilleryEntity.Owner,
        DistilleryType = distilleryEntity.DistilleryType,
        FlavourProfile = FlavourProfileEncoder.Decode(distilleryEntity.EncodedFlavourProfile),
        Active = distilleryEntity.Active
    };
}