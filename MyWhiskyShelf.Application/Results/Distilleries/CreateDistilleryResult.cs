namespace MyWhiskyShelf.Application.Results.Distilleries;

public enum CreateDistilleryOutcome
{
    Created,
    AlreadyExists,
    CountryDoesNotExist,
    RegionDoesNotExistInCountry,
    Error
}

public sealed record CreateDistilleryResult(
    CreateDistilleryOutcome Outcome,
    Core.Aggregates.Distillery? Distillery = null,
    string? Error = null);