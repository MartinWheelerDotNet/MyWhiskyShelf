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
            Name = createDistilleryRequest.Name,
            Location = createDistilleryRequest.Location,
            Region = createDistilleryRequest.Region,
            Founded = createDistilleryRequest.Founded,
            Owner = createDistilleryRequest.Owner,
            Type = createDistilleryRequest.Type,
            EncodedFlavourProfile = FlavourProfileEncoder.Encode(createDistilleryRequest.FlavourProfile),
            Active = createDistilleryRequest.Active
        };
    }
}