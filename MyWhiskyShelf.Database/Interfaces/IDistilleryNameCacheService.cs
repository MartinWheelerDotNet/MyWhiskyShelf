using System.Diagnostics.CodeAnalysis;
using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Contexts;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryNameCacheService
{
    Task InitializeFromDatabaseAsync(MyWhiskyShelfDbContext dbContext);
    IReadOnlyList<DistilleryNameDetails> GetAll();
    bool TryGet(string distilleryName, [NotNullWhen(true)] out DistilleryNameDetails? distilleryNameDetails);
    void Add(string distilleryName, Guid identifier);
    void Remove(string distilleryName);
    IReadOnlyList<DistilleryNameDetails> Search(string queryString);
}