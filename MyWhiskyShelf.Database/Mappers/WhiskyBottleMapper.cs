using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class WhiskyBottleMapper(IDistilleryNameCacheService distilleryNameCacheService)
    : IMapper<WhiskyBottle, WhiskyBottleEntity>
{
    public WhiskyBottle MapToDomain(WhiskyBottleEntity whiskyBottleEntity)
    {
        return new WhiskyBottle
        {
            Name = whiskyBottleEntity.Name,
            DistilleryName = whiskyBottleEntity.DistilleryName,
            Status = Enum.TryParse(whiskyBottleEntity.Status, out BottleStatus status) ? status : BottleStatus.Unknown,
            Bottler = whiskyBottleEntity.Bottler,
            DateBottled = whiskyBottleEntity.DateBottled,
            YearBottled = whiskyBottleEntity.YearBottled ?? whiskyBottleEntity.DateBottled?.Year,
            BatchNumber = whiskyBottleEntity.BatchNumber,
            CaskNumber = whiskyBottleEntity.CaskNumber,
            AbvPercentage = whiskyBottleEntity.AbvPercentage,
            VolumeCl = whiskyBottleEntity.VolumeCl,
            VolumeRemainingCl = whiskyBottleEntity.VolumeRemainingCl,
            AddedColouring = whiskyBottleEntity.AddedColouring,
            ChillFiltered = whiskyBottleEntity.ChillFiltered,
            FlavourProfile = FlavourProfileEncoder.Decode(whiskyBottleEntity.EncodedFlavourProfile)
        };
    }

    public WhiskyBottleEntity MapToEntity(WhiskyBottle whiskyBottle)
    {
        distilleryNameCacheService.TryGet(whiskyBottle.DistilleryName, out var distilleryNameDetails);

        return new WhiskyBottleEntity
        {
            Name = whiskyBottle.Name,
            DistilleryName = whiskyBottle.DistilleryName,
            DistilleryId = distilleryNameDetails?.Identifier,
            Status = whiskyBottle.Status?.ToString() ?? "Unknown",
            Bottler = whiskyBottle.Bottler,
            DateBottled = whiskyBottle.DateBottled,
            YearBottled = whiskyBottle.YearBottled ?? whiskyBottle.DateBottled?.Year,
            BatchNumber = whiskyBottle.BatchNumber,
            CaskNumber = whiskyBottle.CaskNumber,
            AbvPercentage = whiskyBottle.AbvPercentage,
            VolumeCl = whiskyBottle.VolumeCl,
            VolumeRemainingCl = whiskyBottle.VolumeRemainingCl ?? whiskyBottle.VolumeCl,
            AddedColouring = whiskyBottle.AddedColouring,
            ChillFiltered = whiskyBottle.ChillFiltered,
            EncodedFlavourProfile = FlavourProfileEncoder.Encode(whiskyBottle.FlavourProfile)
        };
    }
}