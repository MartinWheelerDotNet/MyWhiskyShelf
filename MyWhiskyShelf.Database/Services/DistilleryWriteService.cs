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
        if (distilleryNameCacheService.TryGet(distillery.DistilleryName, out _))
            return false;
        
        var entity = distilleryMapper.MapToEntity(distillery);
        dbContext.Distilleries.Add(entity);
        distilleryNameCacheService.Add(entity.DistilleryName, entity.Id);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TryRemoveDistilleryAsync(string distilleryName)
    {
        var distilleryEntity = await dbContext.Distilleries
            .FirstOrDefaultAsync(entity => entity.DistilleryName == distilleryName);

        if (distilleryEntity == null) return false;

        distilleryNameCacheService.Remove(distilleryEntity.DistilleryName);
        dbContext.Distilleries.Remove(distilleryEntity);
        await dbContext.SaveChangesAsync();

        return true;
    }
}