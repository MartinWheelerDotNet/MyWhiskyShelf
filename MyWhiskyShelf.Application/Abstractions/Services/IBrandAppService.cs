using MyWhiskyShelf.Application.Results.Brands;

namespace MyWhiskyShelf.Application.Abstractions.Services;

public interface IBrandAppService
{
    Task<GetBrandsResult> GetBrandsAsync(CancellationToken ct = default);
}