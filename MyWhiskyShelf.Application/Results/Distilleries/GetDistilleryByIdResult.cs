namespace MyWhiskyShelf.Application.Results.Distilleries;

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