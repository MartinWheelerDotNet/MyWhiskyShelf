// EF Models should be classes and should have publicly settable properties
// ReSharper disable PropertyCanBeMadeInitOnly.Global
using Pgvector;

namespace MyWhiskyShelf.Infrastructure.Persistence.Entities;

public class DistilleryEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid CountryId { get; set; }
    public Guid? RegionId { get; set; }
    public required int Founded { get; set; }
    public required string Owner { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
    public required string TastingNotes { get; set; }
    public required Vector FlavourVector { get; set; }
    public required bool Active { get; set; }
    
    public CountryEntity Country { get; set; } = null!;
    public RegionEntity? Region { get; set; }
}