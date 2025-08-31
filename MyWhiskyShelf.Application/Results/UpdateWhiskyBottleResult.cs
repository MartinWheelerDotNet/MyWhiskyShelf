using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum UpdateWhiskyBottleOutcome
{
    Updated,
    NotFound,
    Error
}

public sealed record UpdateWhiskyBottleResult(
    UpdateWhiskyBottleOutcome Outcome,
    WhiskyBottle? WhiskyBottle = null,
    string? Error = null);