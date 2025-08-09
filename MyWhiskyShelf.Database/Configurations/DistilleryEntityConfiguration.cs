using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Configurations;

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
        entity.Property(e => e.EncodedFlavourProfile)
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