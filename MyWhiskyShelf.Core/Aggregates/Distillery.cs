using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Core.Aggregates;

public record Distillery
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string Region { get; init; }
    public required int Founded { get; init; }
    public required string Owner { get; init; }
    public required string Type { get; init; }
    public required string Description { get; init; }
    public required string TastingNotes { get; init; }
    public required FlavourProfile FlavourProfile { get; init; } = new();
    public required bool Active { get; init; }
}