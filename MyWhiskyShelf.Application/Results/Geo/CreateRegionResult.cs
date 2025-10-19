using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.Geo;

public enum CreateRegionOutcome
{
    Created,
    CountryNotFound,
    NameConflict,
    Error
}

public sealed record CreateRegionResult(
    CreateRegionOutcome Outcome,
    Region? Region = null,
    string? Error = null);