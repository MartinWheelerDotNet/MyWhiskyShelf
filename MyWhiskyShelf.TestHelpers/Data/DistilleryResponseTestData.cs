using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.TestHelpers.Data;

public static class DistilleryResponseTestData
{
    public static DistilleryResponse Aberargie => new()
    {
        Id = Guid.Parse("00e2b45f-4632-40e4-a29c-8a948fbe85e3"),
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

    public static DistilleryResponse Aberfeldy => new()
    {
        Id = Guid.Parse("9bba2d8b-7c08-4006-a8ef-d858ba7afd78"),
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

    public static DistilleryResponse Bunnahabhain => new()
    {
        Id = Guid.Parse("ea5602a4-91be-4b87-a9aa-1ec0758642f6"),
        DistilleryName = "Bunnahabhain",
        Location = "Port Askaig",
        Region = "Islay",
        Founded = 1881,
        Owner = "Distell",
        DistilleryType = "Malt",
        Active = true
    };

    public static DistilleryResponse Aberlour => new()
    {
        Id = Guid.Parse("1620094e-bd6f-43f5-b289-997b4f160530"),
        DistilleryName = "Aberlour",
        Location = "Aberlour",
        Region = "Speyside",
        Founded = 1879,
        Owner = "Chivas Brothers",
        DistilleryType = "Malt",
        Active = true
    };
}