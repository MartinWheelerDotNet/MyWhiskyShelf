using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Models;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryReadService(MyWhiskyShelfDbContext dbContext)
{
    public async Task<List<Distillery>> GetAllDistilleriesAsync()
        => await dbContext.Distilleries
            .Select(distilleryEntity => new Distillery
            {
                DistilleryName = distilleryEntity.DistilleryName,
                Location = distilleryEntity.Location,
                Region = distilleryEntity.Region,
                Founded = distilleryEntity.Founded,
                Owner = distilleryEntity.Owner,
                DistilleryType = distilleryEntity.DistilleryType,
                Active = distilleryEntity.Active
            })
            .ToListAsync();

    public async Task<List<string>> GetDistilleryNamesAsync() 
        => await dbContext.Distilleries
            .Select(distilleryEntity => distilleryEntity.DistilleryName)
            .ToListAsync();
}