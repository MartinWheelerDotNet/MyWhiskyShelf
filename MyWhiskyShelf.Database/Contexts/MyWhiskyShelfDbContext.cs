using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Database.Models;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Contexts;

public class MyWhiskyShelfDbContext(DbContextOptions<MyWhiskyShelfDbContext> options) : DbContext(options)
{
    private DbSet<DistilleryEntity> Distilleries { get; set; }

    public async Task<List<Distillery>> GetAllDistilleriesAsync()
        => await Distilleries
            .Select(distilleryEntity => distilleryEntity.ProjectToDistillery())
            .ToListAsync();
}