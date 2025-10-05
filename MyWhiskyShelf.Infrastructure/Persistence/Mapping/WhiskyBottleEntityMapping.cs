using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Mapping;

public static class WhiskyBottleEntityMapping
{
    public static WhiskyBottle ToDomain(this WhiskyBottleEntity whiskyBottleEntity)
    {
        return new WhiskyBottle
        {
            Id = whiskyBottleEntity.Id,
            Name = whiskyBottleEntity.Name,
            DistilleryName = whiskyBottleEntity.DistilleryName,
            DistilleryId = whiskyBottleEntity.DistilleryId,
            Status = whiskyBottleEntity.Status,
            Bottler = whiskyBottleEntity.Bottler,
            YearBottled = whiskyBottleEntity.YearBottled,
            BatchNumber = whiskyBottleEntity.BatchNumber,
            CaskNumber = whiskyBottleEntity.CaskNumber,
            AbvPercentage = whiskyBottleEntity.AbvPercentage,
            VolumeCl = whiskyBottleEntity.VolumeCl,
            VolumeRemainingCl = whiskyBottleEntity.VolumeRemainingCl,
            AddedColouring = whiskyBottleEntity.AddedColouring,
            ChillFiltered = whiskyBottleEntity.ChillFiltered,
            FlavourProfile = whiskyBottleEntity.FlavourVector.ToFlavourProfile()
        };
    }

    public static WhiskyBottleEntity ToEntity(this WhiskyBottle whiskyBottle)
    {
        return new WhiskyBottleEntity
        {
            Name = whiskyBottle.Name,
            DistilleryName = whiskyBottle.DistilleryName,
            DistilleryId = whiskyBottle.DistilleryId,
            Status = whiskyBottle.Status,
            Bottler = whiskyBottle.Bottler,
            YearBottled = whiskyBottle.YearBottled,
            BatchNumber = whiskyBottle.BatchNumber,
            CaskNumber = whiskyBottle.CaskNumber,
            AbvPercentage = whiskyBottle.AbvPercentage,
            VolumeCl = whiskyBottle.VolumeCl,
            VolumeRemainingCl = whiskyBottle.VolumeRemainingCl,
            AddedColouring = whiskyBottle.AddedColouring,
            ChillFiltered = whiskyBottle.ChillFiltered,
            FlavourVector = whiskyBottle.FlavourProfile.ToVector()
        };
    }
}