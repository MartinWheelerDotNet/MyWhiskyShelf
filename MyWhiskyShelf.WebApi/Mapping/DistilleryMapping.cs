using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.WebApi.Mapping;

public static class DistilleryMapping
{
    public static DistilleryResponse ToResponse(this Distillery distillery)
    {
        return new DistilleryResponse
        {
            Id = distillery.Id,
            Name = distillery.Name,
            Location = distillery.Location,
            Region = distillery.Region,
            Founded = distillery.Founded,
            Owner = distillery.Owner,
            Type = distillery.Type,
            FlavourProfile = distillery.FlavourProfile,
            Active = distillery.Active
        };
    }

    public static Distillery ToDomain(this DistilleryCreateRequest distilleryCreateRequest)
    {
        return new Distillery
        {
            Name = distilleryCreateRequest.Name,
            Location = distilleryCreateRequest.Location,
            Region = distilleryCreateRequest.Region,
            Founded = distilleryCreateRequest.Founded,
            Owner = distilleryCreateRequest.Owner,
            Type = distilleryCreateRequest.Type,
            FlavourProfile = distilleryCreateRequest.FlavourProfile ?? new FlavourProfile(),
            Active = distilleryCreateRequest.Active
        };
    }

    public static Distillery ToDomain(this DistilleryUpdateRequest distilleryCreateRequest, Guid id)
    {
        return new Distillery
        {
            Id = id,
            Name = distilleryCreateRequest.Name,
            Location = distilleryCreateRequest.Location,
            Region = distilleryCreateRequest.Region,
            Founded = distilleryCreateRequest.Founded,
            Owner = distilleryCreateRequest.Owner,
            Type = distilleryCreateRequest.Type,
            FlavourProfile = distilleryCreateRequest.FlavourProfile ?? new FlavourProfile(),
            Active = distilleryCreateRequest.Active
        };
    }
}