using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class WhiskyBottleWriteService(
    MyWhiskyShelfDbContext dbContext,
    IMapper<WhiskyBottleRequest, WhiskyBottleEntity> mapper) : IWhiskyBottleWriteService
{
   public async Task<(bool hasBeenAdded, Guid? identifier)> TryAddAsync(WhiskyBottleRequest whiskyBottleRequest)
    {
        var whiskyBottleEntity = mapper.Map(whiskyBottleRequest);

        try
        {
            dbContext.WhiskyBottles.Add(whiskyBottleEntity);
            await dbContext.SaveChangesAsync();
            return (true, whiskyBottleEntity.Id);
        }
        catch
        {
            return (false, null);
        }
    }

    public async Task<bool> TryDeleteAsync(Guid identifier)
    {
        var entity = await dbContext.WhiskyBottles.FindAsync(identifier);

        if (entity is null) return false;

        try
        {
            dbContext.WhiskyBottles.Remove(entity);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TryUpdateAsync(Guid identifier, WhiskyBottleRequest whiskyBottleRequest)
    {
        var existingEntity = await dbContext.WhiskyBottles.FindAsync(identifier);
        if (existingEntity is null) return false;
        
        var updatedEntity = mapper.Map(whiskyBottleRequest);
        updatedEntity.Id = existingEntity.Id;
        
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
}