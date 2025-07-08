using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Tests.Resources.TestData;

public static class DistilleryEntityTestData
{
    public static readonly DistilleryEntity Aberargie = new()
    { 
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