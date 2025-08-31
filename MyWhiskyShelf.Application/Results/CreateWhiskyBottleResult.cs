using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum CreateWhiskyBottleOutcome
{
    Created,
    Error
}

public sealed record CreateWhiskyBottleResult(
    CreateWhiskyBottleOutcome Outcome,
    WhiskyBottle? WhiskyBottle = null,
    string? Error = null);