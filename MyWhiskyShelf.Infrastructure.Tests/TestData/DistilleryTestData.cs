using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public static class DistilleryTestData
{
    public static readonly Distillery Generic = new()
    {
        Name = "Name",
        CountryId = Guid.Parse("6551e4a7-4ccb-43f3-85d1-8a0a6f45a1ae"),
        RegionId = Guid.Parse("042861d5-47dd-4324-9beb-f4cc7e9b35c8"),
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        Description = "Description",
        TastingNotes = "TastingNotes",
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };
}