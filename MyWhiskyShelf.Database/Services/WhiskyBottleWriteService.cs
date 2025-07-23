using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class WhiskyBottleWriteService(
    MyWhiskyShelfDbContext dbContext,
    IMapper<WhiskyBottle, WhiskyBottleEntity> mapper) : IWhiskyBottleWriteService
{
    public async Task<bool> TryAddAsync(WhiskyBottle whiskyBottle)
    {
        var whiskyBottleEntity = mapper.MapToEntity(whiskyBottle);
        
        try
        {
            dbContext.WhiskyBottles.Add(whiskyBottleEntity);
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            return false;
        }

        return true;
    }
}