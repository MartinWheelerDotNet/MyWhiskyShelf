using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Tests.TestData;

public static class DistilleryTestData
{
    public static readonly Distillery Generic = new()
    {
        Name = "Name",
        Country = "Country",
        Region = "Region",
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