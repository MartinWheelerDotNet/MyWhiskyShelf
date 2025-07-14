using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class DistilleryMapper : IMapper<Distillery, DistilleryEntity>
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

    public Distillery MapToDomain(DistilleryEntity whiskyBottleEntity) => new()
    {
        DistilleryName = whiskyBottleEntity.DistilleryName,
        Location = whiskyBottleEntity.Location,
        Region = whiskyBottleEntity.Region,
        Founded = whiskyBottleEntity.Founded,
        Owner = whiskyBottleEntity.Owner,
        DistilleryType = whiskyBottleEntity.DistilleryType,
        FlavourProfile = FlavourProfileEncoder.Decode(whiskyBottleEntity.EncodedFlavourProfile),
        Active = whiskyBottleEntity.Active
    };
}