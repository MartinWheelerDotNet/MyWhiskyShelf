namespace MyWhiskyShelf.Infrastructure.Persistence.Entities;

// EF Models should be classes and should have publicly settable properties
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public sealed class RegionEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid CountryId { get; set; }
    public CountryEntity Country { get; set; } = null!;
}