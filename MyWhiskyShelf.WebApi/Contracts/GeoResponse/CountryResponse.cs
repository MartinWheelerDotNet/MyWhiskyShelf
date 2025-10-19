namespace MyWhiskyShelf.WebApi.Contracts.GeoResponse;

public record CountryResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required bool IsActive { get; init; }
    public required List<RegionResponse> Regions { get; init; } = [];
}