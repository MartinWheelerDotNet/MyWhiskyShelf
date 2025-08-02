using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IWhiskyBottleWriteService
{
    Task<(bool hasBeenAdded, Guid? identifier)> TryAddAsync(WhiskyBottleRequest whiskyBottleRequest);
}