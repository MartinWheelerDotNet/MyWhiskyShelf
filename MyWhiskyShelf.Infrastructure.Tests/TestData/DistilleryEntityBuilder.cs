using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public sealed class DistilleryEntityBuilder
{
    private Guid _id;

    public DistilleryEntityBuilder WithId(Guid id) { _id = id; return this; }
    
    public DistilleryEntity Build() => new()
    {
        Id = _id,
        Name = "Name",
        Location = "Location",
        Region = "Region",
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        FlavourProfile = FlavourProfileTestData.Mixed,
        Active = true
    };
}