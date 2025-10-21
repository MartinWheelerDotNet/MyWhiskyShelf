using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.WebApi.Tests.TestData;

public static class DistilleryRequestTestData
{
    public static readonly DistilleryCreateRequest GenericCreateRequest = new()
    {
        Name = "Name",
        CountryId = Guid.NewGuid(),
        RegionId = Guid.NewGuid(),
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        Description = "Description",
        TastingNotes = "TastingNotes",
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };

    public static readonly DistilleryUpdateRequest GenericUpdateRequest = new()
    {
        Name = "Name",
        CountryId = Guid.NewGuid(),
        RegionId = Guid.NewGuid(),
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        Description = "Description",
        TastingNotes = "TastingNotes",
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };
}