namespace MyWhiskyShelf.Application.Results.Geo;

public enum SetRegionActiveOutcome
{
    Updated,
    NotFound,
    Error
}

public sealed record SetRegionActiveResult(
    SetRegionActiveOutcome Outcome,
    string? Error = null);