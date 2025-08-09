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
            .Select(entity => new DistilleryNameDetails(entity.Name, entity.Id))
            .ToListAsync();

        var distilleryNameDetailsDictionary = new ConcurrentDictionary<string, DistilleryNameDetails>(
            distilleryDetails.ToDictionary(details => details.Name, details => details),
            StringComparer.OrdinalIgnoreCase);

        var distilleryIdDictionary = new ConcurrentDictionary<Guid, string>(
            distilleryDetails.ToDictionary(details => details.Id, details => details.Name));

        Interlocked.Exchange(ref _distilleryDetails, distilleryNameDetailsDictionary);
        Interlocked.Exchange(ref _distilleryIds, distilleryIdDictionary);
    }

    public void Add(string distilleryName, Guid id)
    {
        var distilleryNameDetails = new DistilleryNameDetails(distilleryName, id);

        _distilleryDetails.AddOrUpdate(distilleryName, distilleryNameDetails, (_, _) => distilleryNameDetails);
        _distilleryIds.AddOrUpdate(id, distilleryName, (_, _) => distilleryName);
    }

    public void Remove(Guid id)
    {
        if (_distilleryIds.Remove(id, out var distilleryName))
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

        return Process
            .ExtractAll(queryPattern, _distilleryDetails.Keys, cutoff: CutoffRatioForFuzzySearch)
            .OrderByDescending(result => result.Score)
            .ThenBy(result => result.Value, StringComparer.OrdinalIgnoreCase)
            .Select(rankedResult => _distilleryDetails[rankedResult.Value])
            .ToList()
            .AsReadOnly();
    }
}