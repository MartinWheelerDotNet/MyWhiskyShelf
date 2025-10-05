using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public static class DistilleryTestData
{
    public static readonly Distillery Generic = new()
    {
        Name = "Name",
        Country = "Country",
        Region = "Region",
        Founded = 2000,
        Owner = "Owner",
        Type = "Type",
        Description = "Description",
        TastingNotes = "TastingNotes",
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };
}