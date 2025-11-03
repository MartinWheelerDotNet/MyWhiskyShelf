using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results.Distilleries;

public enum GetAllDistilleriesOutcome
{
    Success,
    InvalidCursor,
    Error
}

public sealed record GetAllDistilleriesResult(
    GetAllDistilleriesOutcome Outcome,
    IReadOnlyList<Distillery>? Distilleries = null,
    string? NextCursor = null,
    int Amount = 0,
    string? Error = null);