using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IDistilleryReadRepository
{
    Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Distillery>> SearchByFilter(DistilleryFilterOptions options, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}