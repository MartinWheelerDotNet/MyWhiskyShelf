namespace MyWhiskyShelf.WebApi.Contracts.GeoResponse;

public record CountryCreateRequest
{
    public required string Name { get; init; }
    public required bool IsActive { get; init; }
}