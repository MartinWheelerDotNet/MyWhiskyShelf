using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Configurations;

[ExcludeFromCodeCoverage]
public sealed class CountryEntityConfiguration : IEntityTypeConfiguration<CountryEntity>
{
    public void Configure(EntityTypeBuilder<CountryEntity> b)
    {
        b.HasKey(e => e.Id);
        b.Property(e => e.Id).ValueGeneratedOnAdd();

        b.Property(e => e.Name)
            .HasColumnType("citext")
            .HasMaxLength(50)
            .IsRequired();

        b.Property(e => e.IsActive).IsRequired();

        b.HasIndex(e => e.Name).IsUnique();

        b.HasMany(e => e.Regions)
            .WithOne(r => r.Country)
            .HasForeignKey(r => r.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}