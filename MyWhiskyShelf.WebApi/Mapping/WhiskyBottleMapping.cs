using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;

namespace MyWhiskyShelf.WebApi.Mapping;

public static class WhiskyBottleMapping
{
    public static WhiskyBottleResponse ToResponse(this WhiskyBottle whiskyBottle)
    {
        return new WhiskyBottleResponse
        {
            Id = whiskyBottle.Id,
            Name = whiskyBottle.Name,
            DistilleryName = whiskyBottle.DistilleryName,
            DistilleryId = whiskyBottle.DistilleryId,
            Status = whiskyBottle.Status.ToString(),
            Bottler = whiskyBottle.Bottler,
            YearBottled = whiskyBottle.YearBottled,
            BatchNumber = whiskyBottle.BatchNumber,
            CaskNumber = whiskyBottle.CaskNumber,
            AbvPercentage = whiskyBottle.AbvPercentage,
            VolumeCl = whiskyBottle.VolumeCl,
            VolumeRemainingCl = whiskyBottle.VolumeRemainingCl,
            AddedColouring = whiskyBottle.AddedColouring,
            ChillFiltered = whiskyBottle.ChillFiltered,
            FlavourProfile = whiskyBottle.FlavourProfile
        };
    }

    public static WhiskyBottle ToDomain(this WhiskyBottleCreateRequest whiskyBottleRequest)
    {
        return new WhiskyBottle
        {
            Name = whiskyBottleRequest.Name,
            DistilleryName = whiskyBottleRequest.DistilleryName,
            DistilleryId = whiskyBottleRequest.DistilleryId,
            Status = Enum.TryParse<BottleStatus>(whiskyBottleRequest.Status, ignoreCase: true, out var bottleStatus)
                ? bottleStatus
                : BottleStatus.Unknown,
            Bottler = whiskyBottleRequest.Bottler,
            YearBottled = whiskyBottleRequest.YearBottled,
            BatchNumber = whiskyBottleRequest.BatchNumber,
            CaskNumber = whiskyBottleRequest.CaskNumber,
            AbvPercentage = whiskyBottleRequest.AbvPercentage,
            VolumeCl = whiskyBottleRequest.VolumeCl,
            VolumeRemainingCl = whiskyBottleRequest.VolumeRemainingCl ?? whiskyBottleRequest.VolumeCl,
            AddedColouring = whiskyBottleRequest.AddedColouring,
            ChillFiltered = whiskyBottleRequest.ChillFiltered,
            FlavourProfile = whiskyBottleRequest.FlavourProfile
        };
    }

    public static WhiskyBottle ToDomain(this WhiskyBottleUpdateRequest whiskyBottleRequest)
    {
        return new WhiskyBottle
        {
            Name = whiskyBottleRequest.Name,
            DistilleryName = whiskyBottleRequest.DistilleryName,
            DistilleryId = whiskyBottleRequest.DistilleryId,
            Status = Enum.TryParse<BottleStatus>(whiskyBottleRequest.Status, out var bottleStatus)
                ? bottleStatus
                : BottleStatus.Unknown,
            Bottler = whiskyBottleRequest.Bottler,
            YearBottled = whiskyBottleRequest.YearBottled,
            BatchNumber = whiskyBottleRequest.BatchNumber,
            CaskNumber = whiskyBottleRequest.CaskNumber,
            AbvPercentage = whiskyBottleRequest.AbvPercentage,
            VolumeCl = whiskyBottleRequest.VolumeCl,
            VolumeRemainingCl = whiskyBottleRequest.VolumeRemainingCl ?? whiskyBottleRequest.VolumeCl,
            AddedColouring = whiskyBottleRequest.AddedColouring,
            ChillFiltered = whiskyBottleRequest.ChillFiltered,
            FlavourProfile = whiskyBottleRequest.FlavourProfile
        };
    }
}