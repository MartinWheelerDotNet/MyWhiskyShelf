using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryWriteService(
    MyWhiskyShelfDbContext dbContext,
    IDistilleryNameCacheService distilleryNameCacheService,
    IMapper<DistilleryRequest, DistilleryEntity> mapper) : IDistilleryWriteService
{
    public async Task<bool> TryAddDistilleryAsync(DistilleryRequest distilleryRequest)
    {
        if (distilleryNameCacheService.TryGet(distilleryRequest.DistilleryName, out _)) return false;

        try
        {
            var entity = mapper.Map(distilleryRequest);
            dbContext.Distilleries.Add(entity);
            distilleryNameCacheService.Add(entity.DistilleryName, entity.Id);
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            return false;
        }

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