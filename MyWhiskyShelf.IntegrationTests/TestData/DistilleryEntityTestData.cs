using MyWhiskyShelf.Infrastructure.Persistence.Entities;
using MyWhiskyShelf.Infrastructure.Persistence.Mapping;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class DistilleryEntityTestData
{
    public static DistilleryEntity Generic(
        string name,
        Guid countryId,
        Guid? regionId = null)
    {
        return new DistilleryEntity
        {
            Name = name,
            CountryId = countryId,
            RegionId = regionId,
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            Description = "Description",
            TastingNotes = "TastingNotes",
            FlavourVector = FlavourProfileTestData.Generic.ToVector(),
            Active = true
        };
    }
}