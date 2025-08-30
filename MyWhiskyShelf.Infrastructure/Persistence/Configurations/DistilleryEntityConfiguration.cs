using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWhiskyShelf.Infrastructure.Persistence.Converters;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public class DistilleryEntityConfiguration : IEntityTypeConfiguration<DistilleryEntity>
{
    public void Configure(EntityTypeBuilder<DistilleryEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        entity.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();
        entity.Property(e => e.Location)
            .HasMaxLength(50)
            .IsRequired();
        entity.Property(e => e.Region)
            .HasMaxLength(50)
            .IsRequired();
        entity.Property(e => e.Founded)
            .IsRequired();
        entity.Property(e => e.Owner)
            .HasMaxLength(50)
            .IsRequired();
        entity.Property(e => e.Type)
            .HasMaxLength(25)
            .IsRequired();
        entity.Property(e => e.FlavourProfile)
            .HasConversion<FlavourProfileValueConverter>()
            .HasColumnName("EncodedFlavourProfile")
            .HasColumnType("bigint")
            .IsRequired();
        entity.Property(e => e.Active)
            .IsRequired();

        entity.HasIndex(e => e.Name)
            .IsUnique();
        entity.HasIndex(e => e.Region)
            .IsUnique(false);
        entity.HasIndex(e => e.Owner)
            .IsUnique(false);
        entity.HasIndex(e => e.Type)
            .IsUnique(false);
    }
}