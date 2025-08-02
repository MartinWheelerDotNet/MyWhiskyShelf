using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class WhiskyBottleEntityToResponseMapper : IMapper<WhiskyBottleEntity, WhiskyBottleResponse>
{
    public WhiskyBottleResponse Map(WhiskyBottleEntity whiskyBottleEntity)
    {
        return new WhiskyBottleResponse
        {
            Id = whiskyBottleEntity.Id,
            Name = whiskyBottleEntity.Name,
            DistilleryName = whiskyBottleEntity.DistilleryName,
            Status = whiskyBottleEntity.Status,
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
}