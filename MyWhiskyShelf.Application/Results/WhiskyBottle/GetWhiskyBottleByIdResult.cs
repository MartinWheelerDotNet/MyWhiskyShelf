namespace MyWhiskyShelf.Application.Results.WhiskyBottle;

public enum GetWhiskyBottleByIdOutcome
{
    Success,
    NotFound,
    Error
}

public sealed record GetWhiskyBottleByIdResult(
    GetWhiskyBottleByIdOutcome Outcome,
    Core.Aggregates.WhiskyBottle? WhiskyBottle = null,
    string? Error = null);