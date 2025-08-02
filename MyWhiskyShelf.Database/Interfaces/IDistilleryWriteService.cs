using MyWhiskyShelf.Core.Models;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryWriteService
{
    Task<bool> TryAddDistilleryAsync(DistilleryRequest distilleryRequest);
    Task<bool> TryRemoveDistilleryAsync(string distilleryName);
}