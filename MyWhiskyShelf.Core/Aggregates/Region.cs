namespace MyWhiskyShelf.Core.Aggregates;

public sealed record Region
{
    public Guid Id { get; init; }
    public required Guid CountryId { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required bool IsActive { get; init; }
}
