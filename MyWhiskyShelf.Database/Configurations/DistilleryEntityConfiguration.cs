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
        entity.Property(e => e.DistilleryName)
            .IsRequired();
        entity.Property(e => e.Location)
            .IsRequired();
        entity.Property(e => e.Region)
            .IsRequired();
        entity.Property(e => e.Founded)
            .IsRequired();
        entity.Property(e => e.Owner)
            .IsRequired();
        entity.Property(e => e.DistilleryType)
            .IsRequired();
        entity.Property(e => e.EncodedFlavourProfile)
            .IsRequired();
        entity.Property(e => e.Active)
            .IsRequired();
        entity.HasIndex(e => e.DistilleryName)
            .IsUnique();
        entity.HasIndex(e => e.Region)
            .IsUnique(false);
        entity.HasIndex(e => e.Owner)
            .IsUnique(false);
        entity.HasIndex(e => e.DistilleryType)
            .IsUnique(false);
    }
}