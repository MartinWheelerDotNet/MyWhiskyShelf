using MyWhiskyShelf.Database.Contexts;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryNameCacheService
{
    Task LoadCacheFromDbAsync(MyWhiskyShelfDbContext dbContext);
    List<string> GetAll();
    void Add(string distilleryName);
    List<string> Search(string queryString);
    
}