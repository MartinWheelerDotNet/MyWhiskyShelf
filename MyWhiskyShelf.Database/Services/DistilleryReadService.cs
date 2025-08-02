using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryReadService(
    MyWhiskyShelfDbContext dbContext,
    IMapper<DistilleryEntity, DistilleryResponse> mapper) : IDistilleryReadService
{
    
    public async Task<DistilleryResponse?> GetDistilleryByIdAsync(Guid distilleryId)
    {
        var distillery = await dbContext.Distilleries.FindAsync(distilleryId);

        return distillery is null
            ? null
            : mapper.Map(distillery);
    }
    
    public async Task<IReadOnlyList<DistilleryResponse>> GetAllDistilleriesAsync()
    {
        var distilleryEntities = await dbContext.Distilleries
            .OrderBy(entity => entity.DistilleryName)
            .AsNoTracking()
            .ToListAsync();

        return distilleryEntities.Select(mapper.Map).ToList().AsReadOnly();
    }
}