using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IWhiskyBottleReadService
{
    Task<WhiskyBottleResponse?> GetByIdAsync(Guid distilleryId);
}