namespace MyWhiskyShelf.Core.Aggregates;

public sealed record Brand
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    
    public Guid? CountryId { get; init; }
    public string? CountryName { get; init; }
}