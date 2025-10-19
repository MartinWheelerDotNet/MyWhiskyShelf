namespace MyWhiskyShelf.Application.Results.WhiskyBottle;

public enum DeleteWhiskyBottleOutcome
{
    Deleted,
    NotFound,
    Error
}

public sealed record DeleteWhiskyBottleResult(DeleteWhiskyBottleOutcome Outcome, string? Error = null);