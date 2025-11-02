namespace MyWhiskyShelf.Application.Results.WhiskyBottles;

public enum CreateWhiskyBottleOutcome
{
    Created,
    Error
}

public sealed record CreateWhiskyBottleResult(
    CreateWhiskyBottleOutcome Outcome,
    Core.Aggregates.WhiskyBottle? WhiskyBottle = null,
    string? Error = null);