using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class DistilleryRequestToEntityMapper : IMapper<DistilleryRequest, DistilleryEntity>
{
    public DistilleryEntity Map(DistilleryRequest distilleryRequest)
    {
        return new DistilleryEntity
        {
            DistilleryName = distilleryRequest.DistilleryName,
            Location = distilleryRequest.Location,
            Region = distilleryRequest.Region,
            Founded = distilleryRequest.Founded,
            Owner = distilleryRequest.Owner,
            DistilleryType = distilleryRequest.DistilleryType,
            EncodedFlavourProfile = FlavourProfileEncoder.Encode(distilleryRequest.FlavourProfile),
            Active = distilleryRequest.Active
        };
    }
}