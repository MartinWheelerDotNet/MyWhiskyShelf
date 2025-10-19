using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public class WhiskyBottleEntityConfiguration : IEntityTypeConfiguration<WhiskyBottleEntity>
{
    public void Configure(EntityTypeBuilder<WhiskyBottleEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        builder.Property(e => e.Name)
            .HasColumnType("citext")
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
        builder.Property(e => e.FlavourVector)
            .HasColumnName("FlavourVector")
            .HasColumnType("vector(5)")
            .IsRequired();

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_WhiskyBottles_Name");
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_WhiskyBottles_Name_trgm");
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_WhiskyBottles_Name_eq");
        builder.HasIndex(e => e.DistilleryName)
            .IsUnique(false);
        builder.HasIndex(e => e.Status)
            .IsUnique(false);
        builder.HasIndex(e => e.FlavourVector)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("lists", 100);
    }
}