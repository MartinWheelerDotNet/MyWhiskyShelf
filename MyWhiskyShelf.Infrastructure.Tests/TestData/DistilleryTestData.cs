using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Infrastructure.Tests.TestData;

public static class DistilleryTestData
{
    public static readonly Distillery Generic = new()
    {
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