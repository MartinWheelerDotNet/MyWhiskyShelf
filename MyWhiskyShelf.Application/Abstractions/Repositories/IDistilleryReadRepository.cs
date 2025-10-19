using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IDistilleryReadRepository
{
    Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Distillery>> GetAllAsync(
        string? afterName,
        Guid? afterId,
        int amount,
        CancellationToken ct = default);

    Task<IReadOnlyList<Distillery>> SearchByNameAsync(string pattern, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}