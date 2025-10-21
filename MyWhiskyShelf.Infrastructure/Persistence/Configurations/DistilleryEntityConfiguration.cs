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
        builder.ToTable("Distilleries");
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasColumnType("citext").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Founded).IsRequired();
        builder.Property(e => e.Owner).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Type).HasMaxLength(25).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(250).IsRequired();
        builder.Property(e => e.TastingNotes).HasMaxLength(250).IsRequired();
        builder.Property(e => e.CountryId).IsRequired();
        builder.Property(e => e.RegionId).IsRequired(false);
        builder.Property(e => e.FlavourVector).HasColumnType("vector(5)").IsRequired();
        builder.Property(e => e.Active).IsRequired();
        
        AddRelationships(builder);
        AddStandardIndexes(builder);
        AddFilterIndexes(builder);
    }

    private static void AddRelationships(EntityTypeBuilder<DistilleryEntity> builder)
    {
        builder.HasOne(e => e.Country)
            .WithMany()
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Region)
            .WithMany()
            .HasForeignKey(e => e.RegionId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void AddStandardIndexes(EntityTypeBuilder<DistilleryEntity> builder)
    {
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("UX_Distilleries_Name_eq")
            .IsUnique();
        builder.HasIndex(e => e.FlavourVector)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("lists", 100);
    }

    private static void AddFilterIndexes(EntityTypeBuilder<DistilleryEntity> builder)
    {
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Distilleries_Name_trgm");
        builder.HasIndex(e => e.CountryId)
            .HasDatabaseName("IX_Distilleries_CountryId");
        builder.HasIndex(e => e.RegionId)
            .HasDatabaseName("IX_Distilleries_RegionId");
    }
}