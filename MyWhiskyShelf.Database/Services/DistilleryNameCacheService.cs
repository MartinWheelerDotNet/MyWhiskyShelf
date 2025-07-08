using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryNameCacheService : IDistilleryNameCacheService
{
    private readonly List<string> _distilleryNames = [];
    private const int CutoffRatioForFuzzySearch = 60;
    
    public async Task LoadCacheFromDbAsync(MyWhiskyShelfDbContext dbContext)
    {
        var distilleryNames = await dbContext.Set<DistilleryEntity>()
            .Select(distillery => distillery.DistilleryName)
            .ToListAsync();
        
        _distilleryNames.Clear();
        _distilleryNames.AddRange(distilleryNames);
    }

    public void Add(string distilleryName)
    {
        if (_distilleryNames.Contains(distilleryName)) return;
        _distilleryNames.Add(distilleryName);
    }

    public List<string> GetAll() => _distilleryNames.ToList();

    public List<string> Search(string queryString)
    {
        if (queryString.Length < 3 || string.IsNullOrWhiteSpace(queryString)) return [];

        var rankedResults = Process.ExtractSorted(
            queryString,
            _distilleryNames,
            distilleryName => distilleryName,
            cutoff: CutoffRatioForFuzzySearch);

        return rankedResults.Select(result => result.Value).ToList();
    }
}