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
            Country = distillery.Country,
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
            Country = distilleryCreateRequest.Country,
            Region = distilleryCreateRequest.Region,
            Founded = distilleryCreateRequest.Founded,
            Owner = distilleryCreateRequest.Owner,
            Type = distilleryCreateRequest.Type,
            Description = distilleryCreateRequest.Description,
            TastingNotes = distilleryCreateRequest.TastingNotes,
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
            Country = distilleryCreateRequest.Country,
            Region = distilleryCreateRequest.Region,
            Founded = distilleryCreateRequest.Founded,
            Owner = distilleryCreateRequest.Owner,
            Type = distilleryCreateRequest.Type,
            Description = distilleryCreateRequest.Description,
            TastingNotes = distilleryCreateRequest.TastingNotes,
            FlavourProfile = distilleryCreateRequest.FlavourProfile ?? new FlavourProfile(),
            Active = distilleryCreateRequest.Active
        };
    }
}