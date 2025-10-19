namespace MyWhiskyShelf.Application.Results.Distillery;

public enum CreateDistilleryOutcome
{
    Created,
    AlreadyExists,
    Error
}

public sealed record CreateDistilleryResult(
    CreateDistilleryOutcome Outcome,
    Core.Aggregates.Distillery? Distillery = null,
    string? Error = null);