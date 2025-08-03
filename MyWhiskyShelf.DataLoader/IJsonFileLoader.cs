using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.DataLoader;

public interface IJsonFileLoader
{
    Task<List<CreateDistilleryRequest>> GetDistilleriesFromJsonAsync(string filePath);
}