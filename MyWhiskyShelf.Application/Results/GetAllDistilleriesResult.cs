using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum GetAllDistilleriesOutcome
{
    Success,
    Error
}

public sealed record GetAllDistilleriesResult(
    GetAllDistilleriesOutcome Outcome,
    IReadOnlyList<Distillery>? Distilleries = null,
    int Page = 0,
    int Amount = 0,
    string? Error = null);
