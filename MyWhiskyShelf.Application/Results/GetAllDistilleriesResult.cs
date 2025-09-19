using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum GetAllDistilleryOutcome
{
    Success,
    Error
}

public sealed record GetAllDistilleriesResult(
    GetAllDistilleryOutcome Outcome,
    IReadOnlyList<Distillery>? Distilleries = null,
    string? Error = null);
