namespace MyWhiskyShelf.WebApi.Contracts.GeoResponse;

public record UpdateRegionRequest
{
    public Guid Id { get; init; }
    public required Guid CountryId { get; init; }
    public required string Name { get; init; }
    public required bool IsActive { get; init; }
}