using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public sealed class DistilleryEntityBuilder
{
    private Guid _id;

    public DistilleryEntityBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public DistilleryEntity Build()
    {
        return new DistilleryEntity
        {
            Id = _id,
            Name = "Name",
            CountryId = Guid.NewGuid(),
            RegionId = Guid.NewGuid(),
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            Description = "Description",
            TastingNotes = "TastingNotes",
            FlavourVector = FlavourProfileTestData.GenericVector,
            Active = true
        };
    }
}