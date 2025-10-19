namespace MyWhiskyShelf.Application.Results.Distillery;

public enum SearchDistilleriesOutcome
{
    Success,
    Error
}

public sealed record SearchDistilleriesResult(
    SearchDistilleriesOutcome Outcome,
    IReadOnlyList<Core.Aggregates.Distillery>? Distilleries = null,
    string? Error = null);