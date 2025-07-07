using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Contexts;

// DbSets used by Entity Framework are called via reflection at runtime and so are never accessed in code but
// are required by to be present by Entity Framework.
// ReSharper disable UnusedAutoPropertyAccessor.Global
public class MyWhiskyShelfDbContext(
   DbContextOptions<MyWhiskyShelfDbContext> options) : DbContext(options)
{
    internal DbSet<DistilleryEntity> Distilleries { get; set; }
}