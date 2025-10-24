using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.WebApi.Contracts.Distilleries;

[Serializable]
public record DistilleryCreateRequest
{
    public required string Name { get; init; }
    public required Guid CountryId { get; init; }
    public required string CountryName { get; init; }
    public Guid? RegionId { get; init; }
    public string? RegionName { get; init; }
    public required int Founded { get; init; }
    public required string Owner { get; init; }
    public required string Type { get; init; }
    public required string Description { get; init; }
    public required string TastingNotes { get; init; }
    public FlavourProfile? FlavourProfile { get; init; }
    public required bool Active { get; init; }
}