using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryReadService
{
    Task<DistilleryResponse?> GetDistilleryByIdAsync(Guid distilleryId);
    Task<IReadOnlyList<DistilleryResponse>> GetAllDistilleriesAsync();
}