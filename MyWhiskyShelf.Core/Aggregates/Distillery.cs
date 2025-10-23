using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Core.Aggregates;

public sealed record Distillery
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid CountryId { get; init; }
    public string? CountryName { get; init; }
    public required Guid? RegionId { get; init; }
    public string? RegionName { get; init; }
    public required int Founded { get; init; }
    public required string Owner { get; init; }
    public required string Type { get; init; }
    public required string Description { get; init; }
    public required string TastingNotes { get; init; }
    public required FlavourProfile FlavourProfile { get; init; } = new();
    public required bool Active { get; init; }
}