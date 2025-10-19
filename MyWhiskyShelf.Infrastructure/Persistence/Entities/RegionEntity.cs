using System.Diagnostics.CodeAnalysis;

namespace MyWhiskyShelf.Infrastructure.Persistence.Entities;

[ExcludeFromCodeCoverage]
public sealed class RegionEntity
{
    public Guid Id { get; set; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public Guid CountryId { get; init; }
    public CountryEntity Country { get; init; } = null!;

}