namespace MyWhiskyShelf.Application.Results.WhiskyBottle;

public enum CreateWhiskyBottleOutcome
{
    Created,
    Error
}

public sealed record CreateWhiskyBottleResult(
    CreateWhiskyBottleOutcome Outcome,
    Core.Aggregates.WhiskyBottle? WhiskyBottle = null,
    string? Error = null);