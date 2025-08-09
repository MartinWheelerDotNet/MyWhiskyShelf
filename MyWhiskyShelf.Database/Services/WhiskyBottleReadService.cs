using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class WhiskyBottleReadService(
    MyWhiskyShelfDbContext dbContext,
    IMapper<WhiskyBottleEntity, WhiskyBottleResponse> mapper) : IWhiskyBottleReadService
{
    public async Task<WhiskyBottleResponse?> GetByIdAsync(Guid distilleryId)
    {
        var whiskyBottle = await dbContext.WhiskyBottles.FindAsync(distilleryId);
        
        return whiskyBottle is null
            ? null
            : mapper.Map(whiskyBottle);
    }
}