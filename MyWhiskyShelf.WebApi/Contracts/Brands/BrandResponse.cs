namespace MyWhiskyShelf.WebApi.Contracts.Brands;

public record BrandResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}