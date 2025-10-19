namespace MyWhiskyShelf.Application.Results.Geo;

public enum SetCountryActiveOutcome { Updated, NotFound, Error }

public sealed record SetCountryActiveResult(
    SetCountryActiveOutcome Outcome,
    string? Error = null);