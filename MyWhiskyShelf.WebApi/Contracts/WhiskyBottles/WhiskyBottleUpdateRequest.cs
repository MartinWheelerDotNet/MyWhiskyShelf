using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.WebApi.Contracts.WhiskyBottles;

public record WhiskyBottleUpdateRequest
{
    public required string Name { get; init; }
    public string? DistilleryName { get; init; }
    public Guid? DistilleryId { get; init; }
    public required string? Status { get; init; }
    public string? Bottler { get; init; }
    public int? YearBottled { get; init; }
    public int? BatchNumber { get; init; }
    public int? CaskNumber { get; init; }
    public required decimal AbvPercentage { get; init; }
    public required int VolumeCl { get; init; }
    public int? VolumeRemainingCl { get; init; }
    public bool? AddedColouring { get; init; }
    public bool? ChillFiltered { get; init; }
    public FlavourProfile FlavourProfile { get; init; } = new();
}