using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IWhiskyBottleWriteService
{
    Task<(bool hasBeenAdded, Guid? identifier)> TryAddAsync(WhiskyBottleRequest whiskyBottleRequest);
    Task<bool> TryUpdateAsync(Guid identifier, WhiskyBottleRequest whiskyBottleRequest);
    Task<bool> TryDeleteAsync(Guid identifier);
    Task<bool> TryUpdateAsync(Guid identifier, WhiskyBottleRequest whiskyBottleRequest);
}