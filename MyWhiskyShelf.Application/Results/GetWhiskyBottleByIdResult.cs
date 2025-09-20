using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum GetWhiskyBottleByIdOutcome
{
    Success,
    NotFound,
    Error
}

public sealed record GetWhiskyBottleByIdResult(
    GetWhiskyBottleByIdOutcome Outcome,
    WhiskyBottle? WhiskyBottle = null,
    string? Error = null);
