using Microsoft.Extensions.Logging;
using MyWhiskyShelf.Application.Abstractions.Repositories;
using MyWhiskyShelf.Application.Abstractions.Services;
using MyWhiskyShelf.Application.Results.Brands;

namespace MyWhiskyShelf.Application.Services;

public class BrandAppService(IBrandReadRepository read, ILogger<BrandAppService> logger) : IBrandAppService
{
    public async Task<GetBrandsResult> GetBrandsAsync(CancellationToken ct = default)
    {
        try
        {
            var results = await read.GetBrands(ct);
            return new GetBrandsResult(GetBrandsOutcome.Success, results);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occured when retrieving brands");
            return new GetBrandsResult(GetBrandsOutcome.Error, Error: ex.Message);
        }
    }
}