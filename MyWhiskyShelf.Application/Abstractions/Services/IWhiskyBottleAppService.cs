using MyWhiskyShelf.Application.Results.WhiskyBottles;
using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Services;

public interface IWhiskyBottleAppService
{
    Task<GetWhiskyBottleByIdResult> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CreateWhiskyBottleResult> CreateAsync(WhiskyBottle whiskyBottle, CancellationToken ct = default);
    Task<UpdateWhiskyBottleResult> UpdateAsync(Guid id, WhiskyBottle whiskyBottle, CancellationToken ct = default);
    Task<DeleteWhiskyBottleResult> DeleteAsync(Guid id, CancellationToken ct = default);
}