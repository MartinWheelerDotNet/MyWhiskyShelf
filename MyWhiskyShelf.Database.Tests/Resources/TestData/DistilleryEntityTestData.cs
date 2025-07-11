using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Tests.Resources.TestData;

public static class DistilleryEntityTestData
{
    public static readonly DistilleryEntity Aberargie = new()
    { 
        Id = Guid.Parse("00e2b45f-4632-40e4-a29c-8a948fbe85e3"),
        DistilleryName = "Aberargie",
        Location = "Aberargie",
        Region = "Lowland",
        Founded = 2017,
        Owner = "Perth Distilling Co",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 2449473544ul,
        Active = true
    };
    public static readonly DistilleryEntity Aberfeldy = new()
    {
        Id = Guid.Parse("9bba2d8b-7c08-4006-a8ef-d858ba7afd78"),
        DistilleryName = "Aberfeldy",
        Location = "Aberfeldy",
        Region = "Highland",
        Founded = 1896,
        Owner = "John Dewar & Sons",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 0ul,
        Active = true
    };

    public static readonly DistilleryEntity AbhainnDearg = new()
    {
        Id = Guid.Parse("99046c20-c9e4-426e-b019-97de021964d9"),
        DistilleryName = "Abhainn Dearg",
        Location = "Isle of Lewis",
        Region = "Island",
        Founded = 2008,
        Owner = "Mark Tayburn",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 0ul,
        Active = true
    };

    public static readonly DistilleryEntity Balbalir = new()
    {
        Id = Guid.Parse("3cbd562f-217f-483d-b9a8-17b155fade08"),
        DistilleryName = "Balblair",
        Location = "Edderton",
        Region = "Highland",
        Founded = 1790,
        Owner = "Inver House Distillers",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 0ul,
        Active = true
    };

    public static readonly DistilleryEntity Bunnahabhain = new()
    {
        Id = Guid.Parse("ea5602a4-91be-4b87-a9aa-1ec0758642f6"),
        DistilleryName = "Bunnahabhain",
        Location = "Port Askaig",
        Region = "Islay",
        Founded = 1881,
        Owner = "Distell",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 0ul,
        Active = true
    };
}