using MyWhiskyShelf.Application.Results.Distillery;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Services;

public interface IDistilleryAppService
{
    Task<GetDistilleryByIdResult> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetAllDistilleriesResult> GetAllAsync(int page, int amount, CancellationToken ct = default);
    Task<SearchDistilleriesResult> SearchByNameAsync(string pattern, CancellationToken ct = default);
    Task<CreateDistilleryResult> CreateAsync(Distillery distillery, CancellationToken ct = default);
    Task<UpdateDistilleryResult> UpdateAsync(Guid id, Distillery distillery, CancellationToken ct = default);
    Task<DeleteDistilleryResult> DeleteAsync(Guid id, CancellationToken ct = default);
}