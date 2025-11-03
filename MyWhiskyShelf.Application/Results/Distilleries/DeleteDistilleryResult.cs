namespace MyWhiskyShelf.Application.Results.Distilleries;

public enum DeleteDistilleryOutcome
{
    Deleted,
    NotFound,
    Error
}

public sealed record DeleteDistilleryResult(DeleteDistilleryOutcome Outcome, string? Error = null);