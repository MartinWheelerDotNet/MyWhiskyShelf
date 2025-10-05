using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public sealed class WhiskyBottleEntityBuilder
{
    private Guid _id;

    public WhiskyBottleEntityBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public WhiskyBottleEntity Build()
    {
        return new WhiskyBottleEntity
        {
            Id = _id,
            Name = "Name",
            DistilleryId = Guid.Parse("0b432650-9d62-4331-a060-8d138d4a5720"),
            DistilleryName = "DistilleryName",
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
            FlavourVector = FlavourProfileTestData.GenericVector
        };
    }
}