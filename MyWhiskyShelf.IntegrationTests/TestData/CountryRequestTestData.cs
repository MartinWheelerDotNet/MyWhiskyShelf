using MyWhiskyShelf.WebApi.Contracts.GeoResponse;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class CountryRequestTestData
{
    public static readonly CountryCreateRequest GenericCreate = new()
    {
        Name = "Name",
        IsActive = true
    };
}