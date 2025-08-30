using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class DistilleryRequestTestData
{
    public static readonly DistilleryCreateRequest GenericCreate = new()
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

    public static readonly DistilleryUpdateRequest GenericUpdate = new()
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