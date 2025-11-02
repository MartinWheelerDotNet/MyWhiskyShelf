using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Abstractions.Repositories;

public interface IBrandReadRepository
{
    Task<List<Brand>> GetBrands();
}