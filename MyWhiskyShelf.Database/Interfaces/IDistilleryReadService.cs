using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryReadService
{
    Task<IReadOnlyList<DistilleryResponse>> GetAllDistilleriesAsync();
    Task<DistilleryResponse?> GetDistilleryByNameAsync(string distilleryName);
    IReadOnlyList<DistilleryNameDetails> GetDistilleryNames();
    IReadOnlyList<DistilleryNameDetails> SearchByName(string queryPattern);
}