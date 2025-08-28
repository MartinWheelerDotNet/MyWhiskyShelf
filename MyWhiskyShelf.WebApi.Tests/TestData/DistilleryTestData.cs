using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.WebApi.Tests.TestData;

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
        FlavourProfile = FlavourProfileTestData.Generic,
        Active = true
    };
}