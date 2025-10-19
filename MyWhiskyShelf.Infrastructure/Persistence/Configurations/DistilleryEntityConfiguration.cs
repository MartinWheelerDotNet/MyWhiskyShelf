using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public class DistilleryEntityConfiguration : IEntityTypeConfiguration<DistilleryEntity>
{
    public void Configure(EntityTypeBuilder<DistilleryEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        builder.Property(e => e.Name)
            .HasColumnType("citext")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.Country)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Region)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Founded)
            .IsRequired();
        builder.Property(e => e.Owner)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Type)
            .HasMaxLength(25)
            .IsRequired();
        builder.Property(e => e.Description)
            .HasMaxLength(250)
            .IsRequired();
        builder.Property(e => e.TastingNotes)
            .HasMaxLength(250)
            .IsRequired();
        builder.Property(e => e.FlavourVector)
            .HasColumnName("FlavourVector")
            .HasColumnType("vector(5)")
            .IsRequired();
        builder.Property(e => e.Active)
            .IsRequired();

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Distilleries_Name")
            .IsUnique();
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Distilleries_Name_trgm");
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Distilleries_Name_eq");
        builder.HasIndex(e => new { e.Name, e.Id })
            .HasDatabaseName("IX_Distilleries_Name_Id")
            .IsUnique(false);
        builder.HasIndex(e => e.Region)
            .IsUnique(false);
        builder.HasIndex(e => e.Owner)
            .IsUnique(false);
        builder.HasIndex(e => e.Type)
            .IsUnique(false);
        builder.HasIndex(e => e.FlavourVector)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("lists", 100);
    }
}