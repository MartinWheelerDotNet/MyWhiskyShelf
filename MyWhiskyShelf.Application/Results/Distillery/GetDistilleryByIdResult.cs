namespace MyWhiskyShelf.Application.Results.Distillery;

public enum GetDistilleryByIdOutcome
{
    Success,
    NotFound,
    Error
}

public sealed record GetDistilleryByIdResult(
    GetDistilleryByIdOutcome Outcome,
    Core.Aggregates.Distillery? Distillery = null,
    string? Error = null);