using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryWriteService
{
    Task<(bool hasBeenAdded, Guid? id)> TryAddDistilleryAsync(
        DistilleryRequest distilleryRequest,
        Guid idempotencyKey);
    Task<bool> TryRemoveDistilleryAsync(Guid distilleryId);
}