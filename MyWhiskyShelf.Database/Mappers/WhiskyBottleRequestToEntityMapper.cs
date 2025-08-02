using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class WhiskyBottleRequestToEntityMapper(IDistilleryNameCacheService distilleryNameCacheService)
    : IMapper<WhiskyBottleRequest, WhiskyBottleEntity>
{
    public WhiskyBottleEntity Map(WhiskyBottleRequest whiskyBottleRequest)
    {
        distilleryNameCacheService.TryGet(whiskyBottleRequest.DistilleryName, out var distilleryNameDetails);

        return new WhiskyBottleEntity
        {
            Name = whiskyBottleRequest.Name,
            DistilleryName = whiskyBottleRequest.DistilleryName,
            DistilleryId = distilleryNameDetails?.Identifier,
            Status = whiskyBottleRequest.Status?.ToString() ?? "Unknown",
            Bottler = whiskyBottleRequest.Bottler,
            DateBottled = whiskyBottleRequest.DateBottled,
            YearBottled = whiskyBottleRequest.YearBottled ?? whiskyBottleRequest.DateBottled?.Year,
            BatchNumber = whiskyBottleRequest.BatchNumber,
            CaskNumber = whiskyBottleRequest.CaskNumber,
            AbvPercentage = whiskyBottleRequest.AbvPercentage,
            VolumeCl = whiskyBottleRequest.VolumeCl,
            VolumeRemainingCl = whiskyBottleRequest.VolumeRemainingCl ?? whiskyBottleRequest.VolumeCl,
            AddedColouring = whiskyBottleRequest.AddedColouring,
            ChillFiltered = whiskyBottleRequest.ChillFiltered,
            EncodedFlavourProfile = FlavourProfileEncoder.Encode(whiskyBottleRequest.FlavourProfile)
        };
    }
}