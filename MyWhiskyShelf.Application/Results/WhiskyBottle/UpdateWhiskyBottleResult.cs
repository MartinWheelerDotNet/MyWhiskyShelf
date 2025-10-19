namespace MyWhiskyShelf.Application.Results.WhiskyBottle;

public enum UpdateWhiskyBottleOutcome
{
    Updated,
    NotFound,
    Error
}

public sealed record UpdateWhiskyBottleResult(
    UpdateWhiskyBottleOutcome Outcome,
    Core.Aggregates.WhiskyBottle? WhiskyBottle = null,
    string? Error = null);