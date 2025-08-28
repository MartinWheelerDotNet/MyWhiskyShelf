using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Enums;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

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
        VolumeCl = 75,
        VolumeRemainingCl = 35,
        AddedColouring = true,
        ChillFiltered = true,
        FlavourProfile = FlavourProfileTestData.Mixed
    };
}