using MyWhiskyShelf.Core.Enums;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Core.Aggregates;

public record WhiskyBottle
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? DistilleryName { get; init; }
    public Guid? DistilleryId { get; init; }
    public BottleStatus Status { get; init; }
    public string? Bottler { get; init; }
    public int? YearBottled { get; init; }
    public int? BatchNumber { get; init; }
    public int? CaskNumber { get; init; }
    public required decimal AbvPercentage { get; init; }
    public required int VolumeCl { get; init; }
    public required int VolumeRemainingCl { get; init; }
    public bool? AddedColouring { get; init; }
    public bool? ChillFiltered { get; init; }
    public required FlavourProfile FlavourProfile { get; init; }
}