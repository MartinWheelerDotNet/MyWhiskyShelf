using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.Geo;

public enum UpdateCountryOutcome
{
    Updated,
    NotFound,
    NameConflict,
    Error
}

public sealed record UpdateCountryResult(
    UpdateCountryOutcome Outcome,
    Country? Country = null,
    string? Error = null);