namespace MyWhiskyShelf.Application.Results.Distillery;

public enum GetAllDistilleriesOutcome
{
    Success,
    Error
}

public sealed record GetAllDistilleriesResult(
    GetAllDistilleriesOutcome Outcome,
    IReadOnlyList<Core.Aggregates.Distillery>? Distilleries = null,
    int Page = 0,
    int Amount = 0,
    string? Error = null);
