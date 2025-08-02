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
        }
        catch
        {
            return (false, null);
        }

        return (true, whiskyBottleEntity.Id);
    }
}