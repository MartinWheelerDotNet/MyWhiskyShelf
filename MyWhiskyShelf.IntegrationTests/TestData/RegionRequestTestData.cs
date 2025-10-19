using MyWhiskyShelf.WebApi.Contracts.GeoResponse;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class RegionRequestTestData
{
    public static readonly RegionCreateRequest GenericCreate = new()
    {
        CountryId = Guid.Parse("3b3830b8-081c-4503-8ec4-a623e4cc28bc"),
        Name = "Name",
        IsActive = true
    };
}