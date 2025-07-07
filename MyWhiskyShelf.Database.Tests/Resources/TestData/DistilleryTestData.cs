using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Tests.Resources.TestData;

public static class DistilleryTestData
{
    public static Distillery Aberargie = new()
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
    
    public static Distillery Aberfeldy = new()
    { 
        DistilleryName = "Aberfeldy",
        Location = "Aberfeldy",
        Region = "Highland",
        Founded = 1896,
        Owner = "John Dewar & Sons",
        DistilleryType = "Malt",
        FlavourProfile = new FlavourProfile(),
        Active = true
    };

    public static Distillery AbhainnDearg = new()
    {
        DistilleryName = "Abhainn Dearg",
        Location = "Isle of Lewis",
        Region = "Island",
        Founded = 2008,
        Owner = "Mark Tayburn",
        DistilleryType = "Malt",
        FlavourProfile = new FlavourProfile(),
        Active = true
    };

    public static Distillery Balbalir = new()
    {
        DistilleryName = "Balblair",
        Location = "Edderton",
        Region = "Highland",
        Founded = 1790,
        Owner = "Inver House Distillers",
        DistilleryType = "Malt",
        FlavourProfile = new FlavourProfile(),
        Active = true
    };

    public static Distillery Bunnahabhain = new()
    {
        DistilleryName = "Bunnahabhain",
        Location = "Port Askaig",
        Region = "Islay",
        Founded = 1881,
        Owner = "Distell",
        DistilleryType = "Malt",
        FlavourProfile = new FlavourProfile(),
        Active = true
    };
}