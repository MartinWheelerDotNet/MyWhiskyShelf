using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Tests.TestData;

public static class DistilleryTestData
{
    public static readonly Distillery Generic = new()
    {
        Name = "Name",
        CountryId = Guid.Parse("dcf107a7-8905-470e-8b00-bd552c78bcfc"),
        CountryName = "Country Name",
        RegionId = Guid.Parse("8d860816-5e78-4d51-86f4-ae0cf57d2cf3"),
        RegionName = "Region Name",
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        Description = "Description",
        TastingNotes = "TastingNotes",
        FlavourProfile = new FlavourProfile
        {
            Sweet = 0,
            Fruit = 2,
            Peat = 4,
            Spice = 6,
            Body = 8
        },
        Active = true
    };
}