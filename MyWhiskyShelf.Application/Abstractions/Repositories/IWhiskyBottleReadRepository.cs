using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IWhiskyBottleReadRepository
{
    Task<WhiskyBottle?> GetByIdAsync(Guid id, CancellationToken ct = default);
}