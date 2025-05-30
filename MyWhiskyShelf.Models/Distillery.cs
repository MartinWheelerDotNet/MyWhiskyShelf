namespace MyWhiskyShelf.Models;

[Serializable]
public record Distillery
{
    public required string DistilleryName { get; init; }
    public required string Location { get; init; }
    public required string Region  { get; init; }
    public required int Founded  { get; init; }
    public required string Owner  { get; init; }
    public required string DistilleryType  { get; init; }
    public required bool Active  { get; init; }
}