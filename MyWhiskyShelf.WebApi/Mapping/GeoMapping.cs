using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.WebApi.Contracts.GeoResponse;

namespace MyWhiskyShelf.WebApi.Mapping;

public static class GeoMapping
{
    public static GeoResponse ToResponse(this List<Country> countries)
    {
        return new GeoResponse
        {
            Countries = countries.Select(c => c.ToResponse()).ToList()
        };
    }
}