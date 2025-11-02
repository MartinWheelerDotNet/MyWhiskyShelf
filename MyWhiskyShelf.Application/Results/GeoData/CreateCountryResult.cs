using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.GeoData;

public enum CreateCountryOutcome
{
    Created,
    NameConflict,
    Error
}

public sealed record CreateCountryResult(
    CreateCountryOutcome Outcome,
    Country? Country = null,
    string? Error = null);