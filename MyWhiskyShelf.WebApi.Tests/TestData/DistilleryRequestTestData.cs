using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.WebApi.Tests.TestData;

public static class DistilleryRequestTestData
{
    public static readonly DistilleryCreateRequest GenericCreateRequest = new()
    {
        Name = "Name",
        Location = "Location",
        Region = "Region",
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };
    
    public static readonly DistilleryUpdateRequest GenericUpdateRequest = new()
    {
        Name = "Name",
        Location = "Location",
        Region = "Region",
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };
}