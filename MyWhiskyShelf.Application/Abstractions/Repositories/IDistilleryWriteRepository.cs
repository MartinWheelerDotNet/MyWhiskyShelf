using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IDistilleryWriteRepository
{
    Task<Distillery?> AddAsync(Distillery distillery, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, Distillery distillery, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}