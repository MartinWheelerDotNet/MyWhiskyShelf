// EF Models should be classes and should have publicly settable properties
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MyWhiskyShelf.Database.Entities;

public class DistilleryEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string Region { get; set; }
    public required int Founded { get; set; }
    public required string Owner { get; set; }
    public required string Type { get; set; }
    public required ulong EncodedFlavourProfile { get; set; }
    public required bool Active { get; set; }
}