using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryReadService
{
    Task<List<Distillery>> GetAllDistilleriesAsync();
    Task<Distillery?> GetDistilleryByNameAsync(string distilleryName);
    List<string> GetDistilleryNames();
}