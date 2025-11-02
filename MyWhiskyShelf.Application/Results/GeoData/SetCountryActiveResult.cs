namespace MyWhiskyShelf.Application.Results.GeoData;

public enum SetCountryActiveOutcome
{
    Updated,
    NotFound,
    Error
}

public sealed record SetCountryActiveResult(
    SetCountryActiveOutcome Outcome,
    string? Error = null);