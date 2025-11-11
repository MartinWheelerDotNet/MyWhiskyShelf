namespace MyWhiskyShelf.Infrastructure.Persistence.Entities;

// EF Models should be classes and should have publicly settable properties
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class BrandEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public Guid? CountryId { get; set; }
    public CountryEntity? Country { get; set; }
}