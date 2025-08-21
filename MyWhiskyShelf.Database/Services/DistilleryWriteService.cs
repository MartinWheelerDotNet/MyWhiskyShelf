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
    public async Task<(bool hasBeenAdded, Guid? id)> TryAddDistilleryAsync(DistilleryRequest distilleryRequest)
    {
        if (distilleryNameCacheService.TryGet(distilleryRequest.Name, out _))
            return (false, null);

        try
        {
            var entity = mapper.Map(distilleryRequest);
            dbContext.Distilleries.Add(entity);
            distilleryNameCacheService.Add(entity.Name, entity.Id);
            await dbContext.SaveChangesAsync();
            return (true, entity.Id);
        }
        catch
        {
            return (false, null);
        }
    }

    public async Task<bool> TryUpdateDistilleryAsync(Guid id, DistilleryRequest distilleryRequest)
    {
        var existingEntity = await dbContext.Distilleries.FindAsync(id);
        if (existingEntity is null) return false;

        var updatedEntity = mapper.Map(distilleryRequest);
        updatedEntity.Id = id;

        try
        {
            dbContext.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task RemoveDistilleryAsync(Guid distilleryId)
    {
        var distilleryEntity = await dbContext.Distilleries.FindAsync(distilleryId);

        if (distilleryEntity == null) return;

        distilleryNameCacheService.Remove(distilleryId);
        dbContext.Distilleries.Remove(distilleryEntity);
        await dbContext.SaveChangesAsync();
    }
}