using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IWhiskyBottleWriteService
{
    Task<bool> TryAddAsync(WhiskyBottle whiskyBottle);
}