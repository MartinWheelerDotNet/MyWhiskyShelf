using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Tests.Resources.TestData;

public static class WhiskyBottleTestData
{
    public static readonly WhiskyBottle AllValuesPopulated = new()
    {
        Name = "All Values Populated",
        DistilleryName = "A Distillery Name",
        Status = BottleStatus.Unopened,
        Bottler = "Distillery",
        DateBottled = new DateOnly(2022, 1, 1),
        YearBottled = 2022,
        BatchNumber = 1000,
        CaskNumber = 1,
        AbvPercentage = 50.8m,
        VolumeCl = 70,
        VolumeRemainingCl = 70,
        AddedColouring = false,
        ChillFiltered = false,
        FlavourProfile = new FlavourProfile
        {
            Sweet = 1,
            Fruit = 1,
            Body = 1,
            Tobacco = 2,
            Wine = 2,
            Spice = 3,
            Nut = 1,
            Honey = 1
        }
    };
}