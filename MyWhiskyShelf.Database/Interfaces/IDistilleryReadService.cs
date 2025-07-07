using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryReadService
{
    Task<List<Distillery>> GetAllDistilleriesAsync();
    List<string> GetDistilleryNames();
}