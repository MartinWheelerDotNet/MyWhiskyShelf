using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyWhiskyShelf.Database.Entities;

[Index(nameof(Region), IsUnique = false)]
[Index(nameof(Owner), IsUnique = false)]
[Index(nameof(DistilleryType), IsUnique = false)]
[Index(nameof(Active), IsUnique = false)]
public record DistilleryEntity
{
    [Key]
    public required string DistilleryName { get; init; }
    public required string Location { get; init; }
    public required string Region  { get; init; }
    public required int Founded  { get; init; }
    public required string Owner  { get; init; }
    public required string DistilleryType  { get; init; }
    public required ulong EncodedFlavourProfile { get; init; }
    public required bool Active  { get; init; }
}