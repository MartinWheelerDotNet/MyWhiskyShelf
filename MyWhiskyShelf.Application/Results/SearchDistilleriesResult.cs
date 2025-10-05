using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum SearchDistilleriesOutcome
{
    Success,
    Error
}

public sealed record SearchDistilleriesResult(
    SearchDistilleriesOutcome Outcome,
    IReadOnlyList<Distillery>? Distilleries = null,
    string? Error = null);
