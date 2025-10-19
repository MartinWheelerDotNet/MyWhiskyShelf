namespace MyWhiskyShelf.Application.Results.Distillery;

public enum DeleteDistilleryOutcome
{
    Deleted,
    NotFound,
    Error
}

public sealed record DeleteDistilleryResult(DeleteDistilleryOutcome Outcome, string? Error = null);