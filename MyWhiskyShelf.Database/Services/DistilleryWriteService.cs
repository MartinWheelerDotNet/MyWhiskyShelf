using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryWriteService(
    MyWhiskyShelfDbContext dbContext,
    IDistilleryNameCacheService distilleryNameCacheService,
    IDistilleryMapper distilleryMapper) : IDistilleryWriteService
{
    public async Task<bool> TryAddDistilleryAsync(Distillery distillery)
    {
        if (await dbContext.Distilleries.AnyAsync(entity => distillery.DistilleryName == entity.DistilleryName))
        {
            return false;
        }

        var entity = distilleryMapper.MapToEntity(distillery);
        dbContext.Distilleries.Add(entity);
        distilleryNameCacheService.Add(distillery.DistilleryName);
        await dbContext.SaveChangesAsync();

        return true;
    }
}