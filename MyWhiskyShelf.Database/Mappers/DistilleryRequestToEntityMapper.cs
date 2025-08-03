using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class DistilleryRequestToEntityMapper : IMapper<CreateDistilleryRequest, DistilleryEntity>
{
    public DistilleryEntity Map(CreateDistilleryRequest createDistilleryRequest)
    {
        return new DistilleryEntity
        {
            DistilleryName = createDistilleryRequest.DistilleryName,
            Location = createDistilleryRequest.Location,
            Region = createDistilleryRequest.Region,
            Founded = createDistilleryRequest.Founded,
            Owner = createDistilleryRequest.Owner,
            DistilleryType = createDistilleryRequest.DistilleryType,
            EncodedFlavourProfile = FlavourProfileEncoder.Encode(createDistilleryRequest.FlavourProfile),
            Active = createDistilleryRequest.Active
        };
    }
}