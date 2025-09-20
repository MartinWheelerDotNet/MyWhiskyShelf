using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum GetDistilleryByIdOutcome
{
    Success,
    NotFound,
    Error
}

public sealed record GetDistilleryByIdResult(
    GetDistilleryByIdOutcome Outcome,
    Distillery? Distillery = null,
    string? Error = null);
