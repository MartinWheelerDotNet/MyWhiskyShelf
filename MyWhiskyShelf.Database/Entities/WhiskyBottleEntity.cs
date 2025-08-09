// EF Models should be classes and should have publicly settable properties
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MyWhiskyShelf.Database.Entities;

public class WhiskyBottleEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DistilleryName { get; set; }
    public Guid? DistilleryId { get; set; }
    public required string Status { get; set; }
    public string? Bottler { get; set; }
    public DateOnly? DateBottled { get; set; }
    public int? YearBottled { get; set; }
    public int? BatchNumber { get; set; }
    public int? CaskNumber { get; set; }
    public required decimal AbvPercentage { get; set; }
    public required int VolumeCl { get; set; }
    public required int VolumeRemainingCl { get; set; }
    public bool? AddedColouring { get; set; }
    public bool? ChillFiltered { get; set; }
    public required ulong EncodedFlavourProfile { get; set; }
}