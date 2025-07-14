using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryReadService(
    MyWhiskyShelfDbContext dbContext,
    IDistilleryNameCacheService distilleryNameCacheService,
    IMapper<Distillery, DistilleryEntity> distilleryMapper) : IDistilleryReadService
{
    public async Task<List<Distillery>> GetAllDistilleriesAsync()
        => await dbContext.Distilleries
            .OrderBy(entity => entity.DistilleryName)
            .Select(entity => distilleryMapper.MapToDomain(entity))
            .ToListAsync();
    
    public async Task<Distillery?> GetDistilleryByNameAsync(string distilleryName)
    {
        if (!distilleryNameCacheService.TryGet(distilleryName, out var distilleryDetails))
            return null;
        
        var distillery = await dbContext.Distilleries.FindAsync(distilleryDetails.Identifier);
        
        return distillery is null 
            ? null
            : distilleryMapper.MapToDomain(distillery);
    }

    public List<string> GetDistilleryNames() 
        => distilleryNameCacheService.GetAll().Select(details => details.DistilleryName).ToList();

    public List<string> SearchByName(string query)
        => distilleryNameCacheService.Search(query).Select(details => details.DistilleryName).ToList();
}