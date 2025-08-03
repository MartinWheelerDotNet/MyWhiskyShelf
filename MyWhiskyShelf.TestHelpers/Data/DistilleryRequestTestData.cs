using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.TestHelpers.Data;

public static class DistilleryRequestTestData
{
    public static CreateDistilleryRequest Aberargie => new()
    {
        DistilleryName = "Aberargie",
        Location = "Aberargie",
        Region = "Lowland",
        Founded = 2017,
        Owner = "Perth Distilling Co",
        DistilleryType = "Malt",
        FlavourProfile = new FlavourProfile
        {
            Fruit = 1,
            Spice = 2,
            Malt = 2,
            Nut = 2
        },
        Active = true
    };

    public static CreateDistilleryRequest Aberfeldy => new()
    {
        DistilleryName = "Aberfeldy",
        Location = "Aberfeldy",
        Region = "Highland",
        Founded = 1896,
        Owner = "John Dewar & Sons",
        DistilleryType = "Malt",
        FlavourProfile = new FlavourProfile
        {
            Body = 2,
            Sweet = 2,
            Smoke = 2,
            Medicinal = 0,
            Tobacco = 0,
            Honey = 2,
            Spice = 1,
            Wine = 2,
            Nut = 2,
            Malt = 2,
            Fruit = 2,
            Floral = 2
        },
        Active = true
    };

    public static CreateDistilleryRequest Aberlour => new()
    {
        DistilleryName = "Aberlour",
        Location = "Aberlour",
        Region = "Speyside",
        Founded = 1879,
        Owner = "Chivas Brothers",
        DistilleryType = "Malt",
        Active = true
    };
}