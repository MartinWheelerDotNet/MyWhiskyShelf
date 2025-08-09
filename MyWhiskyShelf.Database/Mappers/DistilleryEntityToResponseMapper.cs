using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Encoders;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Mappers;

public class DistilleryEntityToResponseMapper : IMapper<DistilleryEntity, DistilleryResponse>
{
    public DistilleryResponse Map(DistilleryEntity distilleryEntity)
    {
        return new DistilleryResponse
        {
            Id = distilleryEntity.Id,
            Name = distilleryEntity.Name,
            Location = distilleryEntity.Location,
            Region = distilleryEntity.Region,
            Founded = distilleryEntity.Founded,
            Owner = distilleryEntity.Owner,
            Type = distilleryEntity.Type,
            FlavourProfile = FlavourProfileEncoder.Decode(distilleryEntity.EncodedFlavourProfile),
            Active = distilleryEntity.Active
        };
    }
}