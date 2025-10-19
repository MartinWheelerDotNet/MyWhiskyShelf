namespace MyWhiskyShelf.Application.Results.Distillery;

public enum GetAllDistilleriesOutcome
{
    Success,
    InvalidCursor,
    Error
}

public sealed record GetAllDistilleriesResult(
    GetAllDistilleriesOutcome Outcome,
    IReadOnlyList<Core.Aggregates.Distillery>? Distilleries = null,
    string? NextCursor = null,
    int Amount = 0,
    string? Error = null);