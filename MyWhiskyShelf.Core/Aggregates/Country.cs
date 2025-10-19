namespace MyWhiskyShelf.Core.Aggregates;

public sealed record Country
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required bool IsActive { get; init; }
    public IReadOnlyList<Region> Regions { get; init; } = [];
}
    