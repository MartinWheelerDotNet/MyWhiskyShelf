namespace MyWhiskyShelf.WebApi.Contracts.GeoResponse;

public record GeoResponse
{
    public List<CountryResponse>? Countries { get; init; }
}