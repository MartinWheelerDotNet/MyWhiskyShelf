namespace MyWhiskyShelf.Application.Results.Distillery;

public enum UpdateDistilleryOutcome
{
    Updated,
    NotFound,
    NameConflict,
    CountryDoesNotExist,
    RegionDoesNotExistInCountry,
    Error
}

public sealed record UpdateDistilleryResult(
    UpdateDistilleryOutcome? Outcome,
    Core.Aggregates.Distillery? Distillery = null,
    string? Error = null);