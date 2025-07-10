using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryReadService(
    MyWhiskyShelfDbContext dbContext,
    IDistilleryNameCacheService distilleryNameCacheService,
    IDistilleryMapper distilleryMapper) : IDistilleryReadService
{
    public async Task<List<Distillery>> GetAllDistilleriesAsync()
        => await dbContext.Distilleries
            .Select(entity => distilleryMapper.MapToDomain(entity))
            .ToListAsync();
    
    public async Task<Distillery?> GetDistilleryByNameAsync(string distilleryName)
    {
        var distillery = await dbContext.Distilleries.FindAsync(distilleryName);
        return distillery is null 
            ? null
            : distilleryMapper.MapToDomain(distillery);
    }

    public List<string> GetDistilleryNames() 
        => distilleryNameCacheService.GetAll();

    public List<string> SearchByName(string query)
        => distilleryNameCacheService.Search(query);
}