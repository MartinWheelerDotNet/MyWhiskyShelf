using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class DistilleryRequestTestData
{
    public static DistilleryCreateRequest Create(
        Guid countryId,
        string countryName,
        Guid? regionId = null,
        string? regionName = null,
        string name = "Name")
        => new()
        {
            Name = name,
            CountryId = countryId,
            CountryName = countryName,
            RegionId = regionId,
            RegionName = regionName,
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            Description = "Description",
            TastingNotes = "TastingNotes",
            FlavourProfile = FlavourProfileTestData.Generic,
            Active = true
        };


    public static DistilleryUpdateRequest Update(
        Guid countryId, 
        string countryName, 
        Guid? regionId = null,
        string? regionName = null,
        string name = "Name")
        => new()
        {
            Name = name,
            CountryId = countryId,
            CountryName = countryName,
            RegionId = regionId,
            RegionName = regionName,
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            Description = "Description",
            TastingNotes = "TastingNotes",
            FlavourProfile = FlavourProfileTestData.Generic,
            Active = true
        };
}