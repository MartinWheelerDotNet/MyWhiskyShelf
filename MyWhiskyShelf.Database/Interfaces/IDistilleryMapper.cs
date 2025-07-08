using MyWhiskyShelf.Core.Models;
using MyWhiskyShelf.Database.Entities;

namespace MyWhiskyShelf.Database.Interfaces;

public interface IDistilleryMapper
{
    DistilleryEntity MapToEntity(Distillery distillery);
    Distillery MapToDomain(DistilleryEntity distilleryEntity);
}