using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public sealed class RegionEntityConfiguration : IEntityTypeConfiguration<RegionEntity>
{
    public void Configure(EntityTypeBuilder<RegionEntity> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).ValueGeneratedOnAdd();

        b.Property(e => e.Name)
            .HasColumnType("citext")
            .HasMaxLength(50)
            .IsRequired();

        b.Property(e => e.IsActive).IsRequired();

        b.HasIndex(e => new { e.CountryId, e.Name }).IsUnique();
    }
}