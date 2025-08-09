using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// EF Models should be classes and should have publicly settable properties
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace MyWhiskyShelf.Database.Entities;

[Index(nameof(DistilleryName), IsUnique = true)]
[Index(nameof(Region), IsUnique = false)]
[Index(nameof(Owner), IsUnique = false)]
[Index(nameof(DistilleryType), IsUnique = false)]
[Index(nameof(Active), IsUnique = false)]
public class DistilleryEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string DistilleryName { get; set; }
    public required string Location { get; set; }
    public required string Region { get; set; }
    public required int Founded { get; set; }
    public required string Owner { get; set; }
    public required string DistilleryType { get; set; }
    public required ulong EncodedFlavourProfile { get; set; }
    public required bool Active { get; set; }
}