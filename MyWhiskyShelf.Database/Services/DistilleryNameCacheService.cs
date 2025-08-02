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

    private ConcurrentDictionary<Guid, string> _distilleryIds = new();

    public async Task InitializeFromDatabaseAsync(MyWhiskyShelfDbContext dbContext)
    {
        var distilleryDetails = await dbContext.Set<DistilleryEntity>()
            .OrderBy(entity => entity.DistilleryName)
            .Select(entity => new DistilleryNameDetails(entity.DistilleryName, entity.Id))
            .ToListAsync();

        var distilleryNameDetailsDictionary = new ConcurrentDictionary<string, DistilleryNameDetails>(
            distilleryDetails.ToDictionary(details => details.DistilleryName, details => details),
            StringComparer.OrdinalIgnoreCase);
        
        var distilleryIdDictionary = new ConcurrentDictionary<Guid, string>(
            distilleryDetails.ToDictionary(details => details.Identifier, details => details.DistilleryName));

        Interlocked.Exchange(ref _distilleryDetails, distilleryNameDetailsDictionary);
        Interlocked.Exchange(ref _distilleryIds, distilleryIdDictionary);
    }

    public void Add(string distilleryName, Guid identifier)
    {
        var distilleryNameDetails = new DistilleryNameDetails(distilleryName, identifier);

        _distilleryDetails.AddOrUpdate(distilleryName, distilleryNameDetails, (_, _) => distilleryNameDetails);
        _distilleryIds.AddOrUpdate(identifier, distilleryName, (_, _) => distilleryName);
    }

    public void Remove(Guid identifier)
    {
        if (_distilleryIds.Remove(identifier, out var distilleryName))
            _distilleryDetails.Remove(distilleryName, out _);
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

    public IReadOnlyList<DistilleryNameDetails> Search(string queryPattern)
    {
        if (queryPattern.Length < 3 || string.IsNullOrWhiteSpace(queryPattern)) return [];

        var rankedResults = Process
            .ExtractSorted(
                queryPattern,
                _distilleryDetails.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase),
                cutoff: CutoffRatioForFuzzySearch);

        return rankedResults
            .Select(rankedResult => _distilleryDetails[rankedResult.Value])
            .ToList()
            .AsReadOnly();
    }
}