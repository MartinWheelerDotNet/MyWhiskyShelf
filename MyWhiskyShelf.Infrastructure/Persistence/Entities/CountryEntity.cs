using System.Diagnostics.CodeAnalysis;

namespace MyWhiskyShelf.Infrastructure.Persistence.Entities;

[ExcludeFromCodeCoverage]
public sealed class CountryEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public List<RegionEntity> Regions { get; set; } = [];
    
}