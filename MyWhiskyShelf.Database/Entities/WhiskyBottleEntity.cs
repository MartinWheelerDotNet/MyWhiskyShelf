using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyWhiskyShelf.Database.Entities;

[Index(nameof(DistilleryName), IsUnique = false)]
[Index(nameof(Status), IsUnique = false)]
public record WhiskyBottleEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    public required string Name { get; init; }
    public required string DistilleryName { get; init; }
    public Guid? DistilleryId { get; init; }
    public required string Status { get; init; }
    public string? Bottler { get; init; }
    public DateOnly? DateBottled { get; init; }
    public int? YearBottled { get; init; }
    public int? BatchNumber { get; init; }
    public int? CaskNumber { get; init; }

    [Precision(4, 1)] public required decimal AbvPercentage { get; init; }

    public required int VolumeCl { get; init; }
    public required int VolumeRemainingCl { get; init; }

    public bool? AddedColouring { get; init; }
    public bool? ChillFiltered { get; init; }

    public required ulong EncodedFlavourProfile { get; init; }
}