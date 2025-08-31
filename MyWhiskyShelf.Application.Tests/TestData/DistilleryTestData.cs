using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Tests.TestData;

public static class DistilleryTestData
{
    public static readonly Distillery Generic = new()
    {
        Name = "Name",
        Location = "Location",
        Region = "Region",
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        FlavourProfile = new FlavourProfile
        {
            Sweet = 0,
            Fruit = 1,
            Floral = 2,
            Body = 3,
            Smoke = 4,
            Tobacco = 3,
            Medicinal = 2,
            Wine = 1,
            Spice = 0,
            Malt = 1,
            Nut = 2,
            Honey = 3
        },
        Active = true
    };
}