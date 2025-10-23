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
            CountryId = distillery.CountryId,
            CountryName = distillery.CountryName!,
            RegionId = distillery.RegionId,
            RegionName = distillery.RegionName,
            Founded = distillery.Founded,
            Owner = distillery.Owner,
            Type = distillery.Type,
            Description = distillery.Description,
            TastingNotes = distillery.TastingNotes,
            FlavourProfile = distillery.FlavourProfile,
            Active = distillery.Active
        };
    }

    public static Distillery ToDomain(this DistilleryCreateRequest request)
    {
        return new Distillery
        {
            Name = request.Name,
            CountryId = request.CountryId,
            CountryName = request.CountryName,
            RegionId = request.RegionId,
            RegionName = request.RegionName,
            Founded = request.Founded,
            Owner = request.Owner,
            Type = request.Type,
            Description = request.Description,
            TastingNotes = request.TastingNotes,
            FlavourProfile = request.FlavourProfile ?? new FlavourProfile(),
            Active = request.Active
        };
    }

    public static Distillery ToDomain(this DistilleryUpdateRequest request, Guid id)
    {
        return new Distillery
        {
            Id = id,
            Name = request.Name,
            CountryId = request.CountryId,
            CountryName = request.CountryName,
            RegionId = request.RegionId,
            RegionName = request.RegionName,
            Founded = request.Founded,
            Owner = request.Owner,
            Type = request.Type,
            Description = request.Description,
            TastingNotes = request.TastingNotes,
            FlavourProfile = request.FlavourProfile ?? new FlavourProfile(),
            Active = request.Active
        };
    }
}