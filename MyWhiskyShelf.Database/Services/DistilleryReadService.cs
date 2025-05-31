using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Extensions;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryReadService(MyWhiskyShelfDbContext dbContext)
{
    public async Task<List<Distillery>> GetAllDistilleriesAsync()
        => await dbContext.Distilleries
            .Select(distilleryEntity => distilleryEntity.ProjectToDistillery())
            .ToListAsync();
}