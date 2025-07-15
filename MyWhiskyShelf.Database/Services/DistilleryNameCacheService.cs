using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;
using MyWhiskyShelf.Database.Entities;
using MyWhiskyShelf.Database.Interfaces;

namespace MyWhiskyShelf.Database.Services;

public class DistilleryNameCacheService : IDistilleryNameCacheService
{
    private const int CutoffRatioForFuzzySearch = 60;

    private ConcurrentDictionary<string, DistilleryNameDetails> _distilleryDetails =
        new(StringComparer.OrdinalIgnoreCase);

    public async Task InitializeFromDatabaseAsync(MyWhiskyShelfDbContext dbContext)
    {
        var distilleryDetails = await dbContext.Set<DistilleryEntity>()
            .OrderBy(entity => entity.DistilleryName)
            .Select(entity => new DistilleryNameDetails(entity.DistilleryName, entity.Id))
            .ToListAsync();

        var dictionaryForExchange = new ConcurrentDictionary<string, DistilleryNameDetails>(
            distilleryDetails.ToDictionary(details => details.DistilleryName, details => details),
            StringComparer.OrdinalIgnoreCase);

        Interlocked.Exchange(ref _distilleryDetails, dictionaryForExchange);
    }

    public void Add(string distilleryName, Guid identifier)
    {
        _distilleryDetails
            .TryAdd(distilleryName, new DistilleryNameDetails(distilleryName, identifier));
    }

    public void Remove(string distilleryName)
    {
        _distilleryDetails.TryRemove(distilleryName, out _);
    }

    public IReadOnlyList<DistilleryNameDetails> GetAll()
    {
        return _distilleryDetails
            .OrderBy(details => details.Key)
            .Select(details => details.Value)
            .ToList()
            .AsReadOnly();
    }

    public bool TryGet(string distilleryName, [NotNullWhen(true)] out DistilleryNameDetails? distilleryNameDetails)
    {
        return _distilleryDetails
            .TryGetValue(distilleryName, out distilleryNameDetails);
    }

    public IReadOnlyList<DistilleryNameDetails> Search(string queryString)
    {
        if (queryString.Length < 3 || string.IsNullOrWhiteSpace(queryString)) return [];

        var rankedResults = Process.ExtractSorted(
            queryString,
            _distilleryDetails.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase),
            cutoff: CutoffRatioForFuzzySearch);

        return rankedResults
            .Select(rankedResult => _distilleryDetails[rankedResult.Value])
            .ToList()
            .AsReadOnly();
    }
}