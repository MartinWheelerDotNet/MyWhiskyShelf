using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Tests.Resources;

public static class WhiskyBottleEntityTestData
{
    public static WhiskyBottleEntity AllValuesPopulated => new()
    {
        Id = Guid.Parse("b2901307-8840-45e6-8f4a-a8537d1e5b9a"),
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