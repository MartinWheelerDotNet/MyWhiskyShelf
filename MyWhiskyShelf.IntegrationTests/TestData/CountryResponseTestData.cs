using MyWhiskyShelf.WebApi.Contracts.GeoResponse;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class CountryResponseTestData
{
    public static CountryResponse GenericCreate(Guid id)
    {
        return new CountryResponse
        {
            Id = id,
            Name = "Name",
            IsActive = true,
            Slug = "name",
            Regions = []
        };
            
    }
}