using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class WhiskyBottleWriteService(
    MyWhiskyShelfDbContext dbContext,
    IMapper<WhiskyBottle, WhiskyBottleEntity> mapper)
{
    public async Task<bool> TryAdd(WhiskyBottle whiskyBottle)
    {
        var whiskyBottleEntity = mapper.MapToEntity(whiskyBottle);

        dbContext.WhiskyBottles.Add(whiskyBottleEntity);
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            
            return false;
        }
        
        return true;
    }
}