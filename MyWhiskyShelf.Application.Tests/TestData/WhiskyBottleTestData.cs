using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Tests.TestData;

public static class WhiskyBottleTestData
{
    public static readonly WhiskyBottle Generic = new()
    {
        Name = "Name",
        DistilleryName = "DistilleryName",
        DistilleryId = Guid.Parse("0b432650-9d62-4331-a060-8d138d4a5720"),
        Status = BottleStatus.Opened,
        Bottler = "Bottler",
        YearBottled = 2000,
        BatchNumber = 1234,
        CaskNumber = 10,
        AbvPercentage = 46.3m,
        VolumeCl = 70,
        VolumeRemainingCl = 35,
        AddedColouring = false,
        ChillFiltered = false,
        FlavourProfile = new FlavourProfile
        {
            Sweet = 1,
            Fruit = 3,
            Peat = 5,
            Spice = 7,
            Body = 9
        }
    };
}