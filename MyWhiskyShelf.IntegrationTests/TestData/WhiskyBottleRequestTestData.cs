using MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class WhiskyBottleRequestTestData
{
    public static readonly WhiskyBottleCreateRequest GenericCreate = new()
    {
        Name = "Name",
        DistilleryName = "DistilleryName",
        DistilleryId = Guid.Parse("0b432650-9d62-4331-a060-8d138d4a5720"),
        Status = "Opened",
        Bottler = "Bottler",
        YearBottled = 2000,
        BatchNumber = 1234,
        CaskNumber = 10,
        AbvPercentage = 46.3m,
        VolumeCl = 75,
        VolumeRemainingCl = 35,
        AddedColouring = true,
        ChillFiltered = true,
        FlavourProfile = FlavourProfileTestData.Generic
    };

    public static readonly WhiskyBottleUpdateRequest GenericUpdate = new()
    {
        Name = "Name",
        DistilleryName = "DistilleryName",
        DistilleryId = Guid.Parse("0b432650-9d62-4331-a060-8d138d4a5720"),
        Status = "Opened",
        Bottler = "Bottler",
        YearBottled = 2000,
        BatchNumber = 1234,
        CaskNumber = 10,
        AbvPercentage = 46.3m,
        VolumeCl = 75,
        VolumeRemainingCl = 35,
        AddedColouring = true,
        ChillFiltered = true,
        FlavourProfile = FlavourProfileTestData.Generic
    };
}