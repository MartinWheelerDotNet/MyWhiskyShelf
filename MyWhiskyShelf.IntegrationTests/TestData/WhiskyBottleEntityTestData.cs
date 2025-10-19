using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class WhiskyBottleEntityTestData
{
    public static WhiskyBottleEntity Generic(string name)
    {
        return new WhiskyBottleEntity
        {
            Name = name,
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
            FlavourVector = FlavourProfileTestData.Generic.ToVector()
        };
    }
}