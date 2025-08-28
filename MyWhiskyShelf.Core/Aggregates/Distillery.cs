using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Core.Aggregates;

public record Distillery
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Location { get; init; }
    public required string Region { get; init; }
    public required int Founded { get; init; }
    public required string Owner { get; init; }
    public required string Type { get; init; }
    public required FlavourProfile FlavourProfile { get; init; }
    public required bool Active { get; init; }
}