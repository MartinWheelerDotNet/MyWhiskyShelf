using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Interfaces;
using MyWhiskyShelf.Database.Mappers;

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

    public List<string> GetDistilleryNames() => distilleryNameCacheService.GetAll();
}