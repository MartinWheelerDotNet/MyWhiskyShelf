namespace MyWhiskyShelf.Application.Results.GeoData;

public enum SetRegionActiveOutcome
{
    Updated,
    NotFound,
    Error
}

public sealed record SetRegionActiveResult(
    SetRegionActiveOutcome Outcome,
    string? Error = null);