using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class DistilleryRequestTestData
{
    public static DistilleryCreateRequest Create(Guid countryId, Guid? regionId = null, string name = "Name")
        => new()
        {
            Name = name,
            CountryId = countryId,
            RegionId = regionId,
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            Description = "Description",
            TastingNotes = "TastingNotes",
            FlavourProfile = FlavourProfileTestData.Generic,
            Active = true
        };


    public static DistilleryUpdateRequest Update(Guid countryId, Guid? regionId = null, string name = "Name")
        => new()
        {
            Name = name,
            CountryId = countryId,
            RegionId = regionId,
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            Description = "Description",
            TastingNotes = "TastingNotes",
            FlavourProfile = FlavourProfileTestData.Generic,
            Active = true
        };
}