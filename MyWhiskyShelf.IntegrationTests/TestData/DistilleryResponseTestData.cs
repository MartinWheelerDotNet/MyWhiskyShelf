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
            Location = "Location",
            Region = "Region",
            Founded = 2000,
            Owner = "Owner",
            Type = "Type",
            FlavourProfile = FlavourProfileTestData.Generic,
            Active = true
        };
    }
}