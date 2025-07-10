using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Services;

public interface IDistilleryWriteService
{
    Task<bool> TryAddDistilleryAsync(Distillery distillery);
    Task<bool> TryRemoveDistilleryAsync(string distilleryName);
}