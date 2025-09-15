using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.WebApi.Mapping;

public static class DistilleryNameMapping
{
    public static DistilleryNameResponse ToResponse(this DistilleryName distilleryName)
    {
        return new DistilleryNameResponse(distilleryName.Id, distilleryName.Name);
    }
}