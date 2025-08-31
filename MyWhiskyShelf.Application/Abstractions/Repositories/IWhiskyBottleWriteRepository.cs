using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IWhiskyBottleWriteRepository
{
    Task<WhiskyBottle> AddAsync(WhiskyBottle whiskyBottle, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, WhiskyBottle whiskyBottle, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}