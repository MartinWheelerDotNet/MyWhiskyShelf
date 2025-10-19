using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.Geo;

public enum UpdateRegionOutcome
{
    Updated,
    NotFound,
    CountryChangeAttempted,
    NameConflict,
    Error
}

public sealed record UpdateRegionResult(
    UpdateRegionOutcome Outcome,
    Region? Region = null,
    string? Error = null);