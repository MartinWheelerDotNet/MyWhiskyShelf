using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.DataLoader;

public interface IJsonFileLoader
{
    Task<List<DistilleryRequest>> GetDistilleriesFromJsonAsync(string filePath);
}