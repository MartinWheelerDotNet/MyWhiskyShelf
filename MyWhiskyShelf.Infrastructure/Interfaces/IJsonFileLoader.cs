using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Infrastructure.Interfaces;

public interface IJsonFileLoader
{
    Task<List<Distillery>> GetDistilleriesFromJsonAsync(string filePath, CancellationToken ct = default);
}