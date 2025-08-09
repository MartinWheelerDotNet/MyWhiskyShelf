using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryWriteService
{
    Task<(bool hasBeenAdded, Guid? id)> TryAddDistilleryAsync(CreateDistilleryRequest createDistilleryRequest);
    Task<bool> TryRemoveDistilleryAsync(Guid distilleryId);
}