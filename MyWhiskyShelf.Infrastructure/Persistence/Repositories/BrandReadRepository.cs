using Microsoft.EntityFrameworkCore;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Core.Aggregates;
using MyWhiskyShelf.Infrastructure.Persistence.Contexts;
using MyWhiskyShelf.Infrastructure.Persistence.Projections;

namespace MyWhiskyShelf.Infrastructure.Persistence.Repositories;

public class BrandReadRepository(MyWhiskyShelfDbContext dbContext) : IBrandReadRepository
{
    public async Task<List<Brand>> GetBrands()
    {
        return await dbContext.Brands
            .AsNoTracking()
            .Select(BrandProjections.ToDomain)
            .ToListAsync();
    }
}