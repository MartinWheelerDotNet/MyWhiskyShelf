using MyWhiskyShelf.Core.Aggregates;

namespace MyWhiskyShelf.Application.Results;

public enum CreateDistilleryOutcome
{
    Created,
    AlreadyExists,
    Error
}

public sealed record CreateDistilleryResult(
    CreateDistilleryOutcome Outcome,
    Distillery? Distillery = null,
    string? Error = null);