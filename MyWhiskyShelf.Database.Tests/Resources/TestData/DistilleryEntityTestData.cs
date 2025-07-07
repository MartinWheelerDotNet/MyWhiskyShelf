using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Tests.Resources.TestData;

public static class DistilleryEntityTestData
{
    public static DistilleryEntity Aberargie = new()
    { 
        Id = 1,
        DistilleryName = "Aberargie",
        Location = "Aberargie",
        Region = "Lowland",
        Founded = 2017,
        Owner = "Perth Distilling Co",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 2449473544ul,
        Active = true
    };
    public static DistilleryEntity Aberfeldy = new()
    {
        Id = 2,
        DistilleryName = "Aberfeldy",
        Location = "Aberfeldy",
        Region = "Highland",
        Founded = 1896,
        Owner = "John Dewar & Sons",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 0ul,
        Active = true
    };

    public static DistilleryEntity AbhainnDearg = new()
    {
        Id = 3,
        DistilleryName = "Abhainn Dearg",
        Location = "Isle of Lewis",
        Region = "Island",
        Founded = 2008,
        Owner = "Mark Tayburn",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 0ul,
        Active = true
    };

    public static DistilleryEntity Balbalir = new()
    {
        Id = 4,
        DistilleryName = "Balblair",
        Location = "Edderton",
        Region = "Highland",
        Founded = 1790,
        Owner = "Inver House Distillers",
        DistilleryType = "Malt",
        EncodedFlavourProfile = 0ul,
        Active = true
    };

    public static DistilleryEntity Bunnahabhain = new()
    {
        Id = 5,
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