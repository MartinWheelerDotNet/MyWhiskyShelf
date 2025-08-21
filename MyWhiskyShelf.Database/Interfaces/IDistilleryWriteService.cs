using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryWriteService
{
    Task<(bool hasBeenAdded, Guid? id)> TryAddDistilleryAsync(DistilleryRequest distilleryRequest);
    Task<bool> TryUpdateDistilleryAsync(Guid id, DistilleryRequest distilleryRequest);
    Task RemoveDistilleryAsync(Guid distilleryId);
}