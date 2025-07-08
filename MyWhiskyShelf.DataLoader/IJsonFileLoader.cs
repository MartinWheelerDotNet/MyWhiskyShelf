using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.DataLoader;

public interface IJsonFileLoader
{
    Task<List<Distillery>> GetDistilleriesFromJsonAsync(string filePath);
}