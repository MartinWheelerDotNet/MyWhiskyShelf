namespace MyWhiskyShelf.WebApi.Contracts.GeoResponse;

public record UpdateCountryRequest
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsActive { get; init; }
}