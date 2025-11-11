using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.Brands;

public enum GetBrandsOutcome
{
    Success,
    Error
}

public sealed record GetBrandsResult(
    GetBrandsOutcome Outcome,
    IReadOnlyList<Brand>? Brands = null,
    string? Error = null);