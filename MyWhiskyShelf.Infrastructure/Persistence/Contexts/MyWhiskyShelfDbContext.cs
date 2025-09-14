using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Infrastructure.Persistence.Configurations;
using MyWhiskyShelf.Infrastructure.Persistence.Entities;

namespace MyWhiskyShelf.Infrastructure.Persistence.Contexts;

// DbSets used by Entity Framework are called via reflection at runtime and so are never accessed in code but
// are required by to be present by Entity Framework.
// ReSharper disable UnusedAutoPropertyAccessor.Global
[ExcludeFromCodeCoverage]
public class MyWhiskyShelfDbContext(
    DbContextOptions<MyWhiskyShelfDbContext> options) : DbContext(options)
{
    internal DbSet<DistilleryEntity> Distilleries { get; set; }
    internal DbSet<WhiskyBottleEntity> WhiskyBottles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.HasPostgresExtension("citext");
        
        modelBuilder.ApplyConfiguration(new DistilleryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new WhiskyBottleEntityConfiguration());
    }
}