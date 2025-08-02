using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryWriteService
{
    Task<(bool hasBeenAdded, Guid? identifier)> TryAddDistilleryAsync(DistilleryRequest distilleryRequest);
    Task<bool> TryRemoveDistilleryAsync(Guid distilleryId);
}