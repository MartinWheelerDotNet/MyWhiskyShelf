using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.WebApi.Tests.TestData;

public static class DistilleryTestData
{
    public static readonly Distillery Generic = new()
    {
        Name = "Name",
        CountryId = Guid.NewGuid(),
        CountryName = "Country Name",
        RegionId = Guid.NewGuid(),
        RegionName = "Region Name",
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        Description = "Description",
        TastingNotes = "TastingNotes",
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };
}