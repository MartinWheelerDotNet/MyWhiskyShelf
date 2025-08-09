using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Configurations;

[ExcludeFromCodeCoverage]
public class WhiskyBottleEntityConfiguration : IEntityTypeConfiguration<WhiskyBottleEntity>
{
    public void Configure(EntityTypeBuilder<WhiskyBottleEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.DistilleryName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(e => e.Bottler)
            .HasMaxLength(50);

        builder.Property(e => e.DateBottled);
        builder.Property(e => e.YearBottled);
        builder.Property(e => e.BatchNumber);
        builder.Property(e => e.CaskNumber);

        builder.Property(e => e.AbvPercentage)
            .HasPrecision(4, 1)
            .IsRequired();

        builder.Property(e => e.VolumeCl)
            .IsRequired();

        builder.Property(e => e.VolumeRemainingCl)
            .IsRequired();

        builder.Property(e => e.AddedColouring);
        builder.Property(e => e.ChillFiltered);

        builder.Property(e => e.EncodedFlavourProfile)
            .IsRequired();

        builder.HasIndex(e => e.DistilleryName)
            .IsUnique(false);

        builder.HasIndex(e => e.Status)
            .IsUnique(false);
    }
}