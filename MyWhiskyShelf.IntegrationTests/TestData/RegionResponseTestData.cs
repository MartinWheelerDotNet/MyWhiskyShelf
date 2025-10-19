using MyWhiskyShelf.WebApi.Contracts.GeoResponse;

namespace MyWhiskyShelf.IntegrationTests.TestData;

public static class RegionResponseTestData
{
    public static RegionResponse Generic(Guid id, Guid countryId)
    {
        return new RegionResponse
        {
            Id = id,
            CountryId = countryId,
            Name = "Name",
            Slug = "name",
            IsActive = true
        };
    }
}