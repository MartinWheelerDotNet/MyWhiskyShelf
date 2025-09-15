using MyWhiskyShelf.Application.Results;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Services;

public interface IDistilleryAppService
{
    Task<Distillery?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Distillery>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<DistilleryName>> SearchByNameAsync(string pattern, CancellationToken ct = default);
    Task<CreateDistilleryResult> CreateAsync(Distillery distillery, CancellationToken ct = default);
    Task<UpdateDistilleryResult> UpdateAsync(Guid id, Distillery distillery, CancellationToken ct = default);
    Task<DeleteDistilleryResult> DeleteAsync(Guid id, CancellationToken ct = default);
}