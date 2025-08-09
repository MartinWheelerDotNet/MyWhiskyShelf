using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IWhiskyBottleWriteService
{
    Task<(bool hasBeenAdded, Guid? id)> TryAddAsync(WhiskyBottleRequest whiskyBottleRequest);
    Task<bool> TryUpdateAsync(Guid id, WhiskyBottleRequest whiskyBottleRequest);
    Task<bool> TryDeleteAsync(Guid id);
}