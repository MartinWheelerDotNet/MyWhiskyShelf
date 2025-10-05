using MyWhiskyShelf.WebApi.Contracts.Distilleries;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class DistilleryResponseTestData
{
    public static DistilleryResponse GenericResponse(Guid id)
    {
        return new DistilleryResponse
        {
            Id = id,
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
}