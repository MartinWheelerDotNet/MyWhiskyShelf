using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryWriteService(
    MyWhiskyShelfDbContext dbContext,
    IDistilleryNameCacheService distilleryNameCacheService,
    IMapper<CreateDistilleryRequest, DistilleryEntity> mapper) : IDistilleryWriteService
{
    public async Task<(bool hasBeenAdded, Guid? id)> TryAddDistilleryAsync(
        CreateDistilleryRequest createDistilleryRequest)
    {
        if (distilleryNameCacheService.TryGet(createDistilleryRequest.Name, out _))
            return (false, null);

        try
        {
            var entity = mapper.Map(createDistilleryRequest);
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

    public async Task<bool> TryRemoveDistilleryAsync(Guid distilleryId)
    {
        var distilleryEntity = await dbContext.Distilleries.FindAsync(distilleryId);

        if (distilleryEntity == null) return false;

        distilleryNameCacheService.Remove(distilleryId);
        dbContext.Distilleries.Remove(distilleryEntity);
        await dbContext.SaveChangesAsync();

        return true;
    }
}