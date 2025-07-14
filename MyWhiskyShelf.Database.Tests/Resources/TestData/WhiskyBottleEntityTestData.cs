using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Tests.Resources.TestData;

public static class WhiskyBottleEntityTestData
{
    public static readonly WhiskyBottleEntity AllValuesPopulated = new()
    {
        Name = "All Values Populated",
        DistilleryName = "A Distillery Name",
        Status = "Unopened",
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
        EncodedFlavourProfile = 9718268425ul
    };

}